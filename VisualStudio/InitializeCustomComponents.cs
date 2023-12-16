using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class InitializeCustomComponents
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
    private static class ApplyScriptExtensions
    {
        private static void Postfix(GearItem __instance)
        {
            if (__instance.GetComponent<AmmoItem>() != null && !__instance.name.Contains("GEAR_FlareGunAmmoSingle"))
            {
                _ = __instance.gameObject.GetComponent<AmmoItemExtension>() ?? __instance.gameObject.AddComponent<AmmoItemExtension>();
            }

            if (__instance.GetComponent<GunItem>() != null && !__instance.name.Contains("GEAR_FlareGun"))
            {
                _ = __instance.gameObject.GetComponent<AmmoManager>() ?? __instance.gameObject.AddComponent<AmmoManager>();
                //_ = __instance.gameObject.GetComponent<AttachmentManager>() ?? __instance.gameObject.AddComponent<AttachmentManager>();
            }
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Start))]
    private static class Testing0000
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            if (__instance.GetComponent<vp_FPSShooter>() != null)
            {
                _ = __instance.gameObject.GetComponent<AttachmentManager>() ?? __instance.gameObject.AddComponent<AttachmentManager>();
            }
        }
    }
}