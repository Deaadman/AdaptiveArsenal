using ExtendedWeaponry.Components;

namespace ExtendedWeaponry;

internal class InitializeComponents
{
    [HarmonyPatch(typeof(EquipItemPopup), nameof(EquipItemPopup.Awake))]
    private static class TestingPatch
    {
        private static void Postfix(EquipItemPopup __instance)
        {
            if (__instance.GetComponent<EquipItemPopup>() != null)
            {
                _ = __instance.gameObject.GetComponent<EquipItemPopupAddon>() ?? __instance.gameObject.AddComponent<EquipItemPopupAddon>();
            }
        }
    }

    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
    private static class ApplyScriptExtensions
    {
        private static void Postfix(GearItem __instance)
        {
            if (__instance.GetComponent<AmmoItem>() != null && !__instance.name.Contains("GEAR_FlareGunAmmoSingle"))
            {
                _ = __instance.gameObject.GetComponent<AmmoAddon>() ?? __instance.gameObject.AddComponent<AmmoAddon>();
            }

            if (__instance.GetComponent<GunItem>() != null && !__instance.name.Contains("GEAR_FlareGun"))
            {
                _ = __instance.gameObject.GetComponent<AmmoManager>() ?? __instance.gameObject.AddComponent<AmmoManager>();
                //_ = __instance.gameObject.GetComponent<AttachmentManager>() ?? __instance.gameObject.AddComponent<AttachmentManager>();
            }
        }
    }

    //[HarmonyPatch(typeof(vp_FPSWeapon), nameof(vp_FPSWeapon.Start))]
    //private static class Testing102
    //{
    //    private static void Postfix(vp_FPSWeapon __instance)
    //    {
    //        if (__instance.GetComponent<vp_FPSWeapon>() != null)
    //        {
    //            _ = __instance.gameObject.GetComponent<AttachmentManager>() ?? __instance.gameObject.AddComponent<AttachmentManager>();
    //        }
    //    }
    //}
}