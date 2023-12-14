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
            }
        }
    }
}