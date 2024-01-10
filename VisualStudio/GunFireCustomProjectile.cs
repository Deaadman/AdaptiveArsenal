namespace ExtendedWeaponry;

class GunFireCustomProjectile
{
    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Fire))]
    static class FireCustomProjectile
    {
        static void Prefix(vp_FPSShooter __instance)
        {
            if (Time.time < __instance.m_NextAllowedFireTime || __instance.m_Weapon.ReloadInProgress() || !GameManager.GetPlayerAnimationComponent().IsAllowedToFire(__instance.m_Weapon.m_GunItem.m_AllowHipFire) || GameManager.GetPlayerAnimationComponent().IsReloading() || __instance.m_Weapon.GetAmmoCount() < 1)
            {
                return;
            }

            SetBulletEmissionLocator(__instance);

            if (__instance.ProjectilePrefab.GetComponent<AmmoProjectile>())
            {
                CalculateProjectileTransform(__instance, out Vector3 position, out Quaternion rotation);
                AmmoProjectile.SpawnAndFire(__instance.ProjectilePrefab, position, rotation);
            }
        }

        static void SetBulletEmissionLocator(vp_FPSShooter __instance)
        {
            if (__instance.m_Weapon.m_FirstPersonWeaponRightHand && __instance.m_Weapon.m_FirstPersonWeaponRightHand.m_BulletEmissionPoint)
            {
                __instance.BulletEmissionLocator = __instance.m_Weapon.m_FirstPersonWeaponRightHand.m_BulletEmissionPoint.transform;
            }
            else if (__instance.m_Weapon.m_FirstPersonWeaponShoulder && __instance.m_Weapon.m_FirstPersonWeaponShoulder.m_BulletEmissionPoint)
            {
                __instance.BulletEmissionLocator = __instance.m_Weapon.m_FirstPersonWeaponShoulder.m_BulletEmissionPoint.transform;
            }
        }

        static void CalculateProjectileTransform(vp_FPSShooter __instance, out Vector3 position, out Quaternion rotation)
        {
            Camera weaponCamera = __instance.m_Camera.GetWeaponCamera();
            Camera mainCamera = GameManager.GetMainCamera();
            Vector3 screenPoint = weaponCamera.WorldToScreenPoint(__instance.BulletEmissionLocator.transform.position);
            Vector3 screenPointForward = weaponCamera.WorldToScreenPoint(__instance.BulletEmissionLocator.transform.position + __instance.BulletEmissionLocator.transform.forward);
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
            Vector3 direction = Vector3.Normalize(mainCamera.ScreenToWorldPoint(screenPointForward) - worldPoint);
            position = PlayerManager.MaybeAdjustShotPositionForNearShot(__instance.BulletEmissionLocator.transform.position, worldPoint, direction);
            rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}