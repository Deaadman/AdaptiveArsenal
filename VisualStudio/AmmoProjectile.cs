using ExtendedWeaponry.Utilities;
using Il2CppTLD.Stats;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp(false)]
public class AmmoProjectile : MonoBehaviour
{
    #region References
    private AmmoItem m_AmmoItem;
    private GunExtension m_GunExtension;
    private GunType m_GunType;
    private LineRenderer m_LineRenderer;
    private Rigidbody m_Rigidbody;
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
    private readonly List<Vector3> trajectoryPoints = [];
    private Vector3 m_WindEffect;
    #endregion

    void Awake() => InitializeComponents();

    void ConfigureComponents()
    {
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        m_GunType = m_AmmoItem.m_AmmoForGunType;

        m_LineRenderer = gameObject.AddComponent<LineRenderer>();
        m_LineRenderer.startWidth = m_LineRenderer.endWidth = 0.05f;

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
        Utils.SetIsKinematic(m_Rigidbody, false);
        transform.parent = null;

        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.mass = 0.02f;
        m_Rigidbody.drag = 0.1f;

        Vector3 initialForce = transform.forward * (m_GunExtension.m_MuzzleVelocity * m_ScaleMultiplier) + m_WindEffect;
        m_Rigidbody.AddForce(initialForce, ForceMode.VelocityChange);
    }

    void InitializeComponents()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AmmoItem = GetComponent<AmmoItem>();
        m_GunExtension = GameManager.GetPlayerManagerComponent().m_ItemInHands.GetComponent<GunExtension>();

        ConfigureComponents();
    }

    BaseAi? TryInflictDamage(GameObject victim, string collider)
    {
        BaseAi? baseAi = victim.layer == 16 ? victim.GetComponent<BaseAi>() : victim.layer == 27 ? victim.transform.GetComponentInParent<BaseAi>() : null;
        if (baseAi == null) return null;

        LocalizedDamage localizedDamage = victim.GetComponent<LocalizedDamage>();
        WeaponSource weaponSource = m_GunType.ToWeaponSource();
        float bleedOutMinutes = localizedDamage.GetBleedOutMinutes(weaponSource);
        float damageScale = m_Damage * localizedDamage.GetDamageScale(weaponSource);

        if (!baseAi.m_IgnoreCriticalHits && localizedDamage.RollChanceToKill(WeaponSource.Rifle)) damageScale = float.PositiveInfinity;

        if (baseAi.GetAiMode() != AiMode.Dead)
        {
            if (m_GunType == GunType.Rifle || m_GunType == GunType.Revolver)
            {
                StatsManager.IncrementValue(m_GunType == GunType.Rifle ? StatID.SuccessfulHits_Rifle : StatID.SuccessfulHits_Revolver, 1f);
                GameManager.GetSkillsManager().IncrementPointsAndNotify(m_GunType == GunType.Rifle ? SkillType.Rifle : SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            }
        }

        baseAi.SetupDamageForAnim(transform.position, GameManager.GetPlayerTransform().position, localizedDamage);
        baseAi.ApplyDamage(damageScale, bleedOutMinutes, DamageSource.Player, collider);

        return baseAi;
    }

    void OnCollisionEnter(Collision collision)
    {
        TryInflictDamage(collision.gameObject, collision.gameObject.name);
        ProjectileUtilities.SpawnImpactEffects(collision, transform);
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
            trajectoryPoints.Add(transform.position);
            m_LineRenderer.positionCount = trajectoryPoints.Count;
            m_LineRenderer.SetPositions(trajectoryPoints.ToArray());
        }

        m_Rigidbody.AddForce(m_WindEffect * Time.deltaTime, ForceMode.Acceleration);
    }
}