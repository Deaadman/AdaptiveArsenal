using ExtendedWeaponry.Components;

namespace ExtendedWeaponry.Patches;

internal class ReloadingPatches
{
    [HarmonyPatch(typeof(GunItem), nameof(GunItem.AddRoundsToClip))]
    private static class SingleRoundsToCustomClip
    {
        private static void Postfix(GunItem __instance)
        {
            AmmoManager ammoManager = __instance.gameObject.GetComponent<AmmoManager>();
            ammoManager?.AddRoundsToClip(AmmoManager.m_SelectedAmmoType, 1);
        }
    }

    //[HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
    //private static class RemoveRoundsFromCustomClip
    //{
    //    private static void Postfix(GunItem __instance)
    //    {
    //        AmmoManager ammoManager = __instance.gameObject.GetComponent<AmmoManager>();
    //        ammoManager?.RemoveNextFromClip();
    //    }
    //}

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.OnClipLoaded))]
    private static class MultipleRoundsToCustomClip
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            AmmoManager ammoManager = __instance.m_Weapon.m_GunItem.gameObject.GetComponent<AmmoManager>();
            ammoManager?.AddRoundsToClip(AmmoManager.m_SelectedAmmoType, 5);
        }
    }
}