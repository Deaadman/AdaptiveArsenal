using ExtendedWeaponry.Utilities;
using Utils = ExtendedWeaponry.Utilities.Utils;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
    private static class ApplyBulletTypes
    {
        private static void Postfix(GearItem __instance)
        {
            if (__instance.GetComponent<AmmoItem>() != null)
            {
                _ = __instance.gameObject.GetComponent<AmmoItemExtension>() ?? __instance.gameObject.AddComponent<AmmoItemExtension>();
            }
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetAmmoAvailableForWeapon), new Type[] { typeof(GearItem) })]
    private static class TestingPatch
    {
        private static void Postfix(GearItem weapon, ref int __result, Inventory __instance)
        {
            if (!Utils.IsWeaponValid(weapon))
            {
                Logging.LogError($"Invalid weapon: {weapon.name}");
                return;
            }

            Logging.Log($"Ammo available for {weapon.name}: {__result}");
            int apAmmoCount = 0;
            int standardAmmoCount = 0;

            foreach (var gearItem in __instance.m_Items)
            {
                if (Utils.IsValidAmmo(gearItem, weapon))
                {
                    Utils.UpdateAmmoCounts(gearItem, ref apAmmoCount, ref standardAmmoCount);
                }
            }

            __result = apAmmoCount > 0 ? apAmmoCount : standardAmmoCount;
            Logging.Log($"Ammo selected for {weapon.name}: AP Count = {apAmmoCount}, Standard Count = {standardAmmoCount}");
        }
    }
}