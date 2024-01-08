using ExtendedWeaponry.Utilities;
using Il2CppTLD.Stats;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp(false)]
public class AmmoProjectile : MonoBehaviour
{
    private AmmoItem m_AmmoItem;
    private GunType m_GunType;
    private Rigidbody m_Rigidbody;
    private LineRenderer m_LineRenderer;
    private List<Vector3> trajectoryPoints = [];
    private Vector3 startPosition;
    private Vector3 windEffect;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        m_AmmoItem = GetComponent<AmmoItem>();
        m_GunType = m_AmmoItem.m_AmmoForGunType;
        
        InitializeLineRenderer();
        InitializeWindEffect();
    }

    private void InitializeLineRenderer()
    {
        GameObject lineRendererObject = new("LineRenderer");
        lineRendererObject.transform.SetParent(transform);
        lineRendererObject.transform.localPosition = Vector3.zero;
        lineRendererObject.transform.localRotation = Quaternion.identity;

        m_LineRenderer = lineRendererObject.AddComponent<LineRenderer>();

        m_LineRenderer.startWidth = 0.05f;
        m_LineRenderer.endWidth = 0.05f;
    }

    void Fire()
    {
        Utils.SetIsKinematic(m_Rigidbody, false);
        transform.parent = null;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.mass = 0.2f;
        m_Rigidbody.drag = 0.3f;
        m_Rigidbody.angularDrag = 0.2f;
        m_Rigidbody.centerOfMass = new Vector3(0, 0, 0.0385f);

        Vector3 initialForce = transform.forward * 100 + windEffect;
        m_Rigidbody.AddForce(initialForce, ForceMode.VelocityChange);

        startPosition = transform.position;

        if (m_GunType == GunType.Rifle)
        {
            StatsManager.IncrementValue(StatID.RifleShot, 1f);
        }
        else if (m_GunType == GunType.Revolver)
        {
            StatsManager.IncrementValue(StatID.RevolverShot, 1f);
        }
    }

    private BaseAi InflictDamage(GameObject victim, string collider)
    {
        BaseAi baseAi = null;
        if (victim.layer == 16)
        {
            baseAi = victim.GetComponent<BaseAi>();
        }
        else if (victim.layer == 27)
        {
            baseAi = victim.transform.GetComponentInParent<BaseAi>();
        }
        if (baseAi == null)
        {
            return null;
        }

        LocalizedDamage component = victim.GetComponent<LocalizedDamage>();

        if (m_GunType == GunType.Rifle)
        {
            StatsManager.IncrementValue(StatID.SuccessfulHits_Rifle, 1f);
        }
        else if (m_GunType == GunType.Revolver)
        {
            StatsManager.IncrementValue(StatID.SuccessfulHits_Revolver, 1f);
        }

        WeaponSource weapon = m_GunType.ToWeaponSource();

        float bleedOutMinutes = component.GetBleedOutMinutes(weapon);
        float damage = 100 * component.GetDamageScale(weapon);

        Logging.Log($"Damage: {damage}, Bleedout Minutes: {bleedOutMinutes}, Body Part: {component.m_BodyPart}");

        if (!baseAi.m_IgnoreCriticalHits && component.RollChanceToKill(WeaponSource.Rifle))
        {
            damage = float.PositiveInfinity;
        }
        if (baseAi.GetAiMode() != AiMode.Dead)
        {
            if (m_GunType == GunType.Rifle)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            }
            else if (m_GunType == GunType.Revolver)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            }
        }
        baseAi.SetupDamageForAnim(transform.position, GameManager.GetPlayerTransform().position, component);
        baseAi.ApplyDamage(damage, bleedOutMinutes, DamageSource.Player, collider);
        return baseAi;
    }

    private void InitializeWindEffect()
    {
        Wind windComponent = GameManager.GetWindComponent();
        if (windComponent != null)
        {
            // Convert MPH to meters per second (1 mph ≈ 0.44704 m/s)
            float windSpeedMetersPerSec = windComponent.m_CurrentMPH * 0.44704f;
            windEffect = windComponent.m_CurrentDirection.normalized * windSpeedMetersPerSec;
        }
        else
        {
            windEffect = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collisionGO = collision.gameObject;
        InflictDamage(collisionGO, collisionGO.name);

        SpawnImpactEffects(collision);

        float distanceTravelled = Vector3.Distance(startPosition, transform.position);
        int roundedDistance = Mathf.RoundToInt(distanceTravelled);
        Logging.Log("Projectile traveled distance: " + roundedDistance + " meters");

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

    void SpawnImpactEffects(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        string materialTagForObjectAtPosition = Utils.GetMaterialTagForObjectAtPosition(contact.otherCollider.gameObject, contact.point);
        BulletImpactEffectType impactEffectTypeBasedOnMaterial = vp_Bullet.GetImpactEffectTypeBasedOnMaterial(materialTagForObjectAtPosition);
        BulletImpactEffectPool bulletImpactEffectPool = GameManager.GetEffectPoolManager().GetBulletImpactEffectPool();

        if (impactEffectTypeBasedOnMaterial == BulletImpactEffectType.BulletImpactEffect_Untagged)
        {
            bulletImpactEffectPool.SpawnUntilParticlesDone(BulletImpactEffectType.BulletImpactEffect_Stone, transform.position, transform.rotation);
        }
        else
        {
            bulletImpactEffectPool.SpawnUntilParticlesDone(impactEffectTypeBasedOnMaterial, transform.position, transform.rotation);
        }

        MaterialEffectType materialEffectType = ImpactDecals.MapBulletImpactEffectTypeToMaterialEffectType(impactEffectTypeBasedOnMaterial);
        GameManager.GetDynamicDecalsManager().AddImpactDecal(ProjectileType.Bullet, materialEffectType, contact.point, transform.forward);

        if (contact.otherCollider && contact.otherCollider.gameObject)
        {
            GameAudioManager.SetMaterialSwitch(materialTagForObjectAtPosition, contact.otherCollider.gameObject);
            GameObject soundEmitterFromGameObject = GameAudioManager.GetSoundEmitterFromGameObject(contact.otherCollider.gameObject);
            AkSoundEngine.PostEvent("Play_BulletImpacts", soundEmitterFromGameObject);
            GameAudioManager.SetAudioSourceTransform(contact.otherCollider.gameObject, contact.otherCollider.gameObject.transform);
        }
    }

    void Update()
    {
        if (m_LineRenderer != null)
        {
            trajectoryPoints.Add(transform.position);
            m_LineRenderer.positionCount = trajectoryPoints.Count;
            m_LineRenderer.SetPositions(trajectoryPoints.ToArray());
        }
        m_Rigidbody.AddForce(windEffect * Time.deltaTime, ForceMode.Acceleration);
    }
}