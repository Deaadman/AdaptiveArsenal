using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetAmmoAvailableForWeapon), new Type[] { typeof(GearItem) })]
    private static class NEEDTOTHINKOFANAMEANDPLACETOPUTTHISMETHOD
    {
        private static void Postfix(GearItem weapon, ref int __result, Inventory __instance)
        {
            if (!IsWeaponValid(weapon))
            {
                Logging.LogError($"Invalid weapon: {weapon.name}");
                return;
            }

            //Logging.Log($"Ammo available for {weapon.name}: {__result}");
            int apAmmoCount = 0;
            int standardAmmoCount = 0;

            foreach (var gearItem in __instance.m_Items)
            {
                if (IsValidAmmo(gearItem, weapon))
                {
                    UpdateAmmoCounts(gearItem, ref apAmmoCount, ref standardAmmoCount);
                }
            }

            __result = apAmmoCount > 0 ? apAmmoCount : standardAmmoCount;
            //Logging.Log($"Ammo selected for {weapon.name}: AP Count = {apAmmoCount}, Standard Count = {standardAmmoCount}");
        }

        internal static bool IsWeaponValid(GearItem weapon)
        {
            return weapon != null && weapon.m_GunItem != null;
        }

        internal static bool IsValidAmmo(GearItem gearItem, GearItem weapon)
        {
            return gearItem != null && gearItem.m_AmmoItem && gearItem.m_StackableItem
                   && gearItem.GetRoundedCondition() != 0
                   && gearItem.m_AmmoItem.m_AmmoForGunType == weapon.m_GunItem.m_GunType;
        }

        private static void UpdateAmmoCounts(GearItem gearItem, ref int apAmmoCount, ref int standardAmmoCount)
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

                //Logging.Log($"AmmoItemExtension on {gearItem.name}: m_BulletType = {ammoExtension.m_BulletType}");
            }
        }
    }
}