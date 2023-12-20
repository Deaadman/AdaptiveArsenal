using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Patches;

internal class GunPatches
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Serialize))]
    private class TestingSaving
    {
        private static void Postfix(GearItem __instance)
        {
            AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
            ammoManager?.Serialize();
        }
    }

    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Deserialize))]
    private class TestingLoading
    {
        private static void Postfix(GearItem __instance)
        {
            AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
            ammoManager?.Deserialize();
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetAmmoAvailableForWeapon), new Type[] { typeof(GearItem) })]
    private static class AmmoTypePriority
    {
        private static void Postfix(GearItem weapon, ref int __result, Inventory __instance)
        {
            AmmoManager ammoManager = weapon.m_GunItem.GetComponent<AmmoManager>();
            if (ammoManager == null || ammoManager.GetNextAmmoType() == AmmoType.Unspecified) return;

            var ammoTypeCounts = new Dictionary<AmmoType, int>();

            foreach (var gearItem in __instance.m_Items)
            {
                if (AmmoUtilities.IsValidAmmoType(gearItem, weapon))
                {
                    AmmoUtilities.PrioritizeBulletType(gearItem, ammoTypeCounts);
                }
            }

            __result = ammoTypeCounts.TryGetValue(AmmoType.ArmorPiercing, out int apCount) && apCount > 0 ? apCount : ammoTypeCounts.TryGetValue(AmmoType.FullMetalJacket, out int fullMetalJacketCount) ? fullMetalJacketCount : 0;
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Start))] // Messing around with custom projectiles. This successfully spawns a GEAR_RifleAmmoSingle at the barrel of the weapon.
    private static class SwapCustomProjectiles
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            if (__instance.gameObject.name.Contains("Rifle"))
            {
                GameObject newProjectilePrefab = GearItem.LoadGearItemPrefab("GEAR_RifleAmmoSingle").gameObject;
                if (newProjectilePrefab != null)
                {
                    __instance.ProjectileCustomPrefab = true;
                    __instance.ProjectilePrefab = newProjectilePrefab;
                }
            }
        }
    }

    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.Reload))]
    private static class SwapAmmoMaterials
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            if (__instance.m_Weapon == null || __instance.m_Weapon.m_GearItem == null) return;

            AmmoManager ammoManager = __instance.m_Weapon.m_GearItem.GetComponent<AmmoManager>();
            if (ammoManager == null || ammoManager.GetNextAmmoType() == AmmoType.Unspecified) return;

            Material? nextAmmoMaterial = AmmoUtilities.GetMaterialForAmmoType(ammoManager.GetNextAmmoType());
            if (nextAmmoMaterial == null) return;

            FirstPersonWeapon firstPersonWeapon = __instance.m_Weapon.m_FirstPersonWeaponShoulder.GetComponent<FirstPersonWeapon>();
            if (firstPersonWeapon == null || firstPersonWeapon.m_Renderable == null) return;

            Transform meshesTransform = firstPersonWeapon.m_Renderable.transform.Find("Meshes");
            if (meshesTransform == null) return;

            AmmoUtilities.UpdateAmmoMaterials(meshesTransform, ammoManager, nextAmmoMaterial);
        }
    }
}