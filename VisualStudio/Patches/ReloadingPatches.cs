using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Patches;

internal class ReloadingPatches
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
                AmmoType ammoType = ammoManager.GetNextAmmoType();
                ammoManager.AddRoundsToClip(ammoType);
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
                if (extension.RemoveAmmoFromClip(out AmmoType ammoType))
                {
                    Logging.Log($"Removed bullet from clip: BulletType = {ammoType}");
                }
            }
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
                AmmoType ammoType = ammoManager.GetNextAmmoType();
                ammoManager.AddRoundsToClip(ammoType);
            }
        }
    }
}