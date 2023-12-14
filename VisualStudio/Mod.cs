using ExtendedWeaponry.Utilities;

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
            Logging.Log($"Ammo available for {weapon.name}: {__result}");

            int apAmmoCount = 0;
            int standardAmmoCount = 0;

            for (int i = 0; i < __instance.m_Items.Count; i++)
            {
                GearItem gearItem = __instance.m_Items[i];
                if (gearItem != null && gearItem.m_AmmoItem && gearItem.m_StackableItem
                    && gearItem.GetRoundedCondition() != 0
                    && gearItem.m_AmmoItem.m_AmmoForGunType == weapon.m_GunItem.m_GunType)
                {
                    AmmoItemExtension ammoExtension = gearItem.GetComponent<AmmoItemExtension>();
                    if (ammoExtension != null)
                    {
                        if (ammoExtension.m_BulletType == BulletType.ArmorPiercing)
                        {
                            apAmmoCount += gearItem.m_StackableItem.m_Units;
                        }
                        else if (ammoExtension.m_BulletType == BulletType.Standard)
                        {
                            standardAmmoCount += gearItem.m_StackableItem.m_Units;
                        }

                        Logging.Log($"AmmoItemExtension on {gearItem.name}: m_BulletType = {ammoExtension.m_BulletType}");
                    }
                    else
                    {
                        Logging.Log($"No AmmoItemExtension found on {gearItem.name}");
                    }
                }
            }

            if (apAmmoCount > 0)
            {
                __result = apAmmoCount;
                Logging.Log($"Prioritizing Armor Piercing ammo, Count: {apAmmoCount}");
            }
            else
            {
                __result = standardAmmoCount;
                Logging.Log($"Using Standard ammo, Count: {standardAmmoCount}");
            }
        }
    }

    [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
    private static class Testing
    {
    }
}