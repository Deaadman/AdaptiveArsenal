namespace ExtendedWeaponry.Utilities;

internal class ProjectileUtilities
{
    internal static void SpawnImpactEffects(Collision collision, Transform transform)
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
}