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
                _ = __instance.gameObject.GetComponent<AttachmentManager>() ?? __instance.gameObject.AddComponent<AttachmentManager>();
            }
        }
    }

    [HarmonyPatch(typeof(FirstPersonWeapon), nameof(FirstPersonWeapon.EnableRenderable))]
    private static class Testing102
    {
        private static void Postfix(FirstPersonWeapon __instance)
        {
            if (__instance.GetComponent<FirstPersonWeapon>() != null)
            {
                _ = __instance.gameObject.GetComponent<AttachmentManager>() ?? __instance.gameObject.AddComponent<AttachmentManager>();
            }
        }
    }
}