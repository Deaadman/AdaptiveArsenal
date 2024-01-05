using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;
using Il2CppNodeCanvas.BehaviourTrees;
using static Il2Cpp.UITweener;

namespace ExtendedWeaponry.Patches;

internal class GunPatches
{
    //[HarmonyPatch(typeof(GearItem), nameof(GearItem.Serialize))]
    //private static class TestingSaving
    //{
    //    private static void Postfix(GearItem __instance)
    //    {
    //        AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
    //        ammoManager?.Serialize();
    //    }
    //}

    //[HarmonyPatch(typeof(GearItem), nameof(GearItem.Deserialize))]
    //private static class TestingLoading
    //{
    //    private static void Postfix(GearItem __instance)
    //    {
    //        AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
    //        ammoManager?.Deserialize();
    //    }
    //}

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetAmmoAvailableForWeapon), new Type[] { typeof(GearItem) })]
    internal static class AmmoTypePriority
    {
        private static bool Prefix(ref int __result, GearItem weapon)
        {
            AmmoManager ammoManager = weapon.m_GunItem.GetComponent<AmmoManager>();
            if (ammoManager != null)
            {
                __result = AmmoManager.CalculateAmmoAvailableForType();
                return false;
            }
            return true;
        }
    }

    //[HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Start))] // Messing around with custom projectiles. This successfully spawns a GEAR_RifleAmmoSingle at the barrel of the weapon.
    //private static class SwapCustomProjectiles
    //{
    //    private static void Postfix(vp_FPSShooter __instance)
    //    {
    //        if (__instance.gameObject.name.Contains("Rifle"))
    //        {
    //            GameObject newProjectilePrefab = GearItem.LoadGearItemPrefab("GEAR_RifleAmmoSingle").gameObject;
    //            if (newProjectilePrefab != null)
    //            {
    //                __instance.ProjectileCustomPrefab = true;
    //                __instance.ProjectilePrefab = newProjectilePrefab;
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Reload))]
    //private static class SwapAmmoMaterials
    //{
    //    private static void Postfix(vp_FPSShooter __instance)
    //    {
    //        if (__instance.m_Weapon == null || __instance.m_Weapon.m_GearItem == null) return;

    //        AmmoManager ammoManager = __instance.m_Weapon.m_GearItem.GetComponent<AmmoManager>();
    //        if (ammoManager == null) return;

    //        Material? nextAmmoMaterial = AmmoUtilities.GetMaterialForAmmoType(AmmoManager.m_SelectedAmmoType);
    //        if (nextAmmoMaterial == null) return;

    //        FirstPersonWeapon firstPersonWeapon = __instance.m_Weapon.m_FirstPersonWeaponShoulder.GetComponent<FirstPersonWeapon>();
    //        if (firstPersonWeapon == null || firstPersonWeapon.m_Renderable == null) return;

    //        Transform meshesTransform = firstPersonWeapon.m_Renderable.transform.Find("Meshes");
    //        if (meshesTransform == null) return;

    //        AmmoUtilities.UpdateAmmoMaterials(meshesTransform, ammoManager, nextAmmoMaterial);
    //    }
    //}
}