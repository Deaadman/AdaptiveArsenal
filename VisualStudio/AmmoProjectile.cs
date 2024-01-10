using ExtendedWeaponry.Utilities;
using Il2CppTLD.Stats;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp(false)]
public class AmmoProjectile : MonoBehaviour
{
    #region References
#nullable disable
    private AmmoItem m_AmmoItem;
    private GunExtension m_GunExtension;
    private GunType m_GunType;
    private LineRenderer m_LineRenderer;
    private Rigidbody m_Rigidbody;
#nullable enable
    #endregion

    #region Adjustable Values
    /// <summary>
    /// A float which multiplies against the muzzle velocity for guns.
    /// </summary>
    private readonly float m_ScaleMultiplier = 0.15f;

    /// <summary>
    /// A float which determines how affective the wind is against the projectiles.
    /// </summary>
    private readonly float m_WindEffectMultiplier = 1;

    /// <summary>
    /// The damage of the weapon (All weapons are 100 by default).
    /// </summary>
    private readonly float m_Damage = 100;
    #endregion

    #region Other
    private readonly List<Vector3> m_TrajectoryPoints = [];
    private Vector3 m_WindEffect;
    #endregion

    void Awake() => InitializeComponents();

    void ConfigureComponents()
    {
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        m_GunType = m_AmmoItem.m_AmmoForGunType;

        Material? fxArrowTrailMaterial = TextureSwapper.GetLineRendererMaterialFromGearItemPrefab("GEAR_Arrow", "LineRenderer");
        if (fxArrowTrailMaterial != null)
        {
            GameObject lineRendererObject = new("LineRenderer");
            lineRendererObject.transform.parent = transform;

            m_LineRenderer = lineRendererObject.AddComponent<LineRenderer>();
            m_LineRenderer.material = fxArrowTrailMaterial;
            m_LineRenderer.startWidth = m_LineRenderer.endWidth = 0.1f;

            Gradient gradient = new();
            gradient.SetKeys(
                new GradientColorKey[] { new(Color.white, 0.0f), new(Color.white, 1.0f) },
                new GradientAlphaKey[] { new(1.0f, 0.0f), new(0.0f, 1.0f) }
            );
            m_LineRenderer.colorGradient = gradient;
        }

        Wind windComponent = GameManager.GetWindComponent();
        if (windComponent != null)
        {
            float windSpeedMetersPerSec = windComponent.m_CurrentMPH * 0.44704f;
            m_WindEffect = windComponent.m_CurrentDirection.normalized * windSpeedMetersPerSec * m_WindEffectMultiplier;
        }
        else
        {
            m_WindEffect = Vector3.zero;
        }
    }

    void Fire()
    {
        StatsManager.IncrementValue(m_GunType == GunType.Rifle ? StatID.SuccessfulHits_Rifle : StatID.SuccessfulHits_Revolver, 1f);

        Utils.SetIsKinematic(m_Rigidbody, false);
        transform.parent = null;

        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.mass = 0.02f;
        m_Rigidbody.drag = 0.1f;
        m_Rigidbody.angularDrag = 0.1f;

        m_LineRenderer.startColor = new Color(1f, 1f, 1f, 0f);
        m_LineRenderer.endColor = Color.white * 0.7f;

        Vector3 initialForce = transform.forward * (m_GunExtension.m_MuzzleVelocity * m_ScaleMultiplier) + m_WindEffect;
        m_Rigidbody.AddForce(initialForce, ForceMode.VelocityChange);
    }

    void InitializeComponents()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AmmoItem = GetComponent<AmmoItem>();
        if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
        {
            m_GunExtension = GameManager.GetPlayerManagerComponent().m_ItemInHands.GetComponent<GunExtension>();
        }

        ConfigureComponents();
    }

    BaseAi? TryInflictDamage(GameObject victim, string collider)
    {
        BaseAi? baseAi = victim.layer == 16 ? victim.GetComponent<BaseAi>() : victim.layer == 27 ? victim.transform.GetComponentInParent<BaseAi>() : null;
        if (baseAi == null) return null;

        LocalizedDamage localizedDamage = victim.GetComponent<LocalizedDamage>();
        WeaponSource weaponSource = m_GunType.ToWeaponSource();
        float bleedOutMinutes = localizedDamage.GetBleedOutMinutes(weaponSource);
        float damageScaleFactor = localizedDamage.GetDamageScale(weaponSource);
        float damage = m_Damage * damageScaleFactor;

        if (!baseAi.m_IgnoreCriticalHits && localizedDamage.RollChanceToKill(WeaponSource.Rifle)) damage = float.PositiveInfinity;

        if (baseAi.GetAiMode() != AiMode.Dead)
        {
            StatID statId = m_GunType == GunType.Rifle ? StatID.SuccessfulHits_Rifle : StatID.SuccessfulHits_Revolver;
            SkillType skillType = m_GunType == GunType.Rifle ? SkillType.Rifle : SkillType.Revolver;
            StatsManager.IncrementValue(statId, 1f);
            GameManager.GetSkillsManager().IncrementPointsAndNotify(skillType, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
        }

        baseAi.SetupDamageForAnim(transform.position, GameManager.GetPlayerTransform().position, localizedDamage);
        baseAi.ApplyDamage(damage, bleedOutMinutes, DamageSource.Player, collider);

        return baseAi;
    }

    void OnCollisionEnter(Collision collision)
    {
        TryInflictDamage(collision.gameObject, collision.gameObject.name);

        ProjectileUtilities.SpawnImpactEffects(collision, transform);
        //Transform childTransform = transform.Find("DecalRenderer");
        //Renderer renderer = childTransform.GetComponent<Renderer>();
        //if (renderer != null && renderer.enabled)
        //{
        //    vp_DecalManager.Add(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
        Destroy(gameObject);
    }

    internal static GameObject SpawnAndFire(GameObject prefab, Vector3 startPos, Quaternion startRot)
    {
        GameObject gameObject = Instantiate(prefab, startPos, startRot);
        gameObject.name = prefab.name;
        gameObject.transform.parent = null;
        gameObject.GetComponent<AmmoProjectile>().Fire();
        return gameObject;
    }

    void Update()
    {
        if (m_LineRenderer != null)
        {
            m_TrajectoryPoints.Add(transform.position);
            m_LineRenderer.positionCount = m_TrajectoryPoints.Count;
            m_LineRenderer.SetPositions(m_TrajectoryPoints.ToArray());
        }

        m_Rigidbody.AddForce(m_WindEffect * Time.deltaTime, ForceMode.Acceleration);
    }
}