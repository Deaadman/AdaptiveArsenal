using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class GunItemMechanics
{
    [HarmonyPatch(typeof(GunItem), nameof(GunItem.AddRoundsToClip))]
    private static class SingleRoundsToCustomClip
    {
        private static void Postfix(GunItem __instance, int count)
        {
            AmmoManager extension = __instance.GetComponent<AmmoManager>();
            if (extension == null) return;

            for (int i = 0; i < count; i++)
            {
                BulletType bulletType = extension.GetNextBulletType();
                extension.AddRoundsToClip(bulletType);
            }
        }
    }

    [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
    private static class RemoveRoundsFromCustomClip
    {
        private static void Prefix(GunItem __instance)
        {
            AmmoManager extension = __instance.GetComponent<AmmoManager>();
            if (extension != null)
            {
                if (extension.RemoveNextFromClip(out AmmoManager.BulletInfo bulletInfo))
                {
                    Logging.Log($"Removed bullet from clip: BulletType = {bulletInfo.m_BulletType}");
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

            __result = bulletTypeCounts.TryGetValue(BulletType.ArmorPiercing, out int apCount) && apCount > 0 ? apCount
                      : bulletTypeCounts.TryGetValue(BulletType.Standard, out int standardCount) ? standardCount
                      : 0;
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