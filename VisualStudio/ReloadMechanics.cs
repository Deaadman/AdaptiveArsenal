using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class ReloadMechanics
{
    [HarmonyPatch(typeof(GunItem), nameof(GunItem.AddRoundsToClip))]
    private static class SingleRoundsToCustomClip
    {
        private static void Postfix(GunItem __instance, int count)
        {
            AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            for (int i = 0; i < count; i++)
            {
                BulletType bulletType = ammoManager.GetNextBulletType();
                ammoManager.AddRoundsToClip(bulletType);
            }
        }
    }

    //[HarmonyPatch(typeof(GearItem), nameof(GearItem.Serialize))]
    //private class TestingSaving
    //{
    //    private static void Postfix(GearItem __instance)
    //    {
    //        AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
    //        ammoManager?.SaveAmmoData();
    //    }
    //}

    [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
    private static class RemoveRoundsFromCustomClip
    {
        private static void Prefix(GunItem __instance)
        {
            AmmoManager extension = __instance.GetComponent<AmmoManager>();
            if (extension != null)
            {
                if (extension.RemoveNextFromClip(out BulletType bulletType))
                {
                    Logging.Log($"Removed bullet from clip: BulletType = {bulletType}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetAmmoAvailableForWeapon), new Type[] { typeof(GearItem) })]
    private static class AmmoTypePriority
    {
        private static void Postfix(GearItem weapon, ref int __result, Inventory __instance)
        {
            AmmoManager ammoManager = weapon.m_GunItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            var bulletTypeCounts = new Dictionary<BulletType, int>();

            foreach (var gearItem in __instance.m_Items)
            {
                if (AmmoManager.IsValidAmmo(gearItem, weapon))
                {
                    AmmoManager.PrioritizeBulletType(gearItem, bulletTypeCounts);
                }
            }

            __result = bulletTypeCounts.TryGetValue(BulletType.ArmorPiercing, out int apCount) && apCount > 0 ? apCount : bulletTypeCounts.TryGetValue(BulletType.FullMetalJacket, out int fullMetalJacketCount) ? fullMetalJacketCount : 0;
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Reload))]
    private static class SwapBulletMaterials
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            if (__instance.m_Weapon == null || __instance.m_Weapon.m_GearItem == null) return;

            AmmoManager ammoManager = __instance.m_Weapon.m_GearItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            Material? nextBulletMaterial = AmmoManager.GetMaterialForBulletType(ammoManager.GetNextBulletType());
            if (nextBulletMaterial == null) return;

            FirstPersonWeapon firstPersonWeapon = __instance.m_Weapon.m_FirstPersonWeaponShoulder.GetComponent<FirstPersonWeapon>();
            if (firstPersonWeapon == null || firstPersonWeapon.m_Renderable == null) return;

            Transform meshesTransform = firstPersonWeapon.m_Renderable.transform.Find("Meshes");
            if (meshesTransform == null) return;

            AmmoManager.UpdateBulletMaterials(meshesTransform, ammoManager, nextBulletMaterial);
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.OnClipLoaded))]
    private static class MultipleRoundsToCustomClip
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            GunItem? gunItem = __instance.m_Weapon.m_GunItem;
            if (gunItem == null) return;

            AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            int bulletsToAdd = Math.Min(5, gunItem.m_ClipSize - ammoManager.m_Clip.Count);

            for (int i = 0; i < bulletsToAdd; i++)
            {
                BulletType bulletType = ammoManager.GetNextBulletType();
                ammoManager.AddRoundsToClip(bulletType);
            }
        }
    }
}