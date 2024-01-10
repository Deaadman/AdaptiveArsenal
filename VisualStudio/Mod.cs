using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    //[HarmonyPatch(typeof(vp_Bullet), nameof(vp_Bullet.SpawnImpactEffects))]
    //static class LogBulletData
    //{
    //   static bool Prefix(vp_Bullet __instance, RaycastHit hit)
    //    {
    //        Vector3 playerPosition = GameManager.GetVpFPSPlayer().transform.position;
    //        Vector3 collisionPoint = hit.point;
    //        float distance = Vector3.Distance(playerPosition, collisionPoint);

    //        WeaponSource weaponSource = __instance.m_GunType.ToWeaponSource();
    //        BodyDamage bodyDamage = hit.collider.GetComponentInParent<BodyDamage>();

    //        if (bodyDamage != null)
    //        {
    //            LocalizedDamage localizedDamage = hit.collider.GetComponent<LocalizedDamage>();
    //            if (localizedDamage != null)
    //            {
    //                BodyPart hitBodyPart = localizedDamage.m_BodyPart;
    //                float damageScale = bodyDamage.GetDamageScale(hitBodyPart, weaponSource);
    //                float originalDamage = __instance.Damage;
    //                float actualDamage = originalDamage * damageScale;

    //                Logging.Log($"Bullet Impact Detected. Body Part: {hitBodyPart}, Distance: {distance}, Original Damage: {originalDamage}, Damage Multiplier: {damageScale}, Actual Damage: {actualDamage}");
    //            }
    //        }
    //        else
    //        {
    //            Logging.Log($"Bullet Impact Detected. Distance: {distance}, but BodyDamage component not found on collider's parent.");
    //        }

    //        return true;
    //    }
    //}
}