using UnityEngine.AddressableAssets;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
    private static class ApplyComponents
    {
        private static void Postfix(GearItem __instance)
        {
            if (__instance.GetComponent<GunItem>() != null)
            {
                _ = __instance.gameObject.GetComponent<GunExtension>() ?? __instance.gameObject.AddComponent<GunExtension>();
            }
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Awake))]
    private static class SwapCustomProjectiles
    {
        private static void Prefix(vp_FPSShooter __instance)
        {
            if (__instance.gameObject.name.Contains("Rifle"))
            {
                GameObject newProjectilePrefab = Addressables.LoadAsset<GameObject>("GEAR_RifleAmmoSingle").WaitForCompletion();
                if (newProjectilePrefab != null)
                {
                    _ = newProjectilePrefab.GetComponent<AmmoProjectile>() ?? newProjectilePrefab.AddComponent<AmmoProjectile>();
                    __instance.ProjectileCustomPrefab = true;
                    __instance.ProjectilePrefab = newProjectilePrefab;
                }
            }
            if (__instance.gameObject.name.Contains("Revolver"))
            {
                GameObject newProjectilePrefab = Addressables.LoadAsset<GameObject>("GEAR_RevolverAmmoSingle").WaitForCompletion();
                if (newProjectilePrefab != null)
                {
                    _ = newProjectilePrefab.GetComponent<AmmoProjectile>() ?? newProjectilePrefab.AddComponent<AmmoProjectile>();
                    __instance.ProjectileCustomPrefab = true;
                    __instance.ProjectilePrefab = newProjectilePrefab;
                }
            }
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Fire))] // First shot, shoots down and towards the left?
    private static class FireCustomProjectile
    {
        private static void Prefix(vp_FPSShooter __instance)
        {
            if (Time.time < __instance.m_NextAllowedFireTime)
            {
                return;
            }
            if (__instance.m_Weapon.ReloadInProgress())
            {
                return;
            }
            if (!GameManager.GetPlayerAnimationComponent().IsAllowedToFire(__instance.m_Weapon.m_GunItem.m_AllowHipFire))
            {
                return;
            }
            if (GameManager.GetPlayerAnimationComponent().IsReloading())
            {
                return;
            }
            if (__instance.m_Weapon.GetAmmoCount() < 1)
            {
                return;
            }
            Camera weaponCamera = __instance.m_Camera.GetWeaponCamera();
            Camera mainCamera = GameManager.GetMainCamera();
            Vector3 vector = __instance.m_Camera.transform.position;
            Quaternion quaternion = __instance.m_Camera.transform.rotation;
            if (__instance.BulletEmissionLocator != null)
            {
                Vector3 vector8 = weaponCamera.WorldToScreenPoint(__instance.BulletEmissionLocator.transform.position);
                Vector3 vector9 = weaponCamera.WorldToScreenPoint(__instance.BulletEmissionLocator.transform.position + __instance.BulletEmissionLocator.transform.forward);
                Vector3 vector10 = mainCamera.ScreenToWorldPoint(vector8);
                Vector3 vector11 = Vector3.Normalize(mainCamera.ScreenToWorldPoint(vector9) - vector10);
                vector = vector10;
                quaternion = Quaternion.LookRotation(vector11, Vector3.up);
                Vector3 vector12 = vector11;
                vector = PlayerManager.MaybeAdjustShotPositionForNearShot(__instance.BulletEmissionLocator.transform.position, vector, vector12);
            }
            if (__instance.ProjectileCustomPrefab)
            {
                if (__instance.ProjectilePrefab.GetComponent<AmmoProjectile>() != null)
                {
                    AmmoProjectile.SpawnAndFire(__instance.ProjectilePrefab, vector, quaternion);
                }
            }
        }
    }
}