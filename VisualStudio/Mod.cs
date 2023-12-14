using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
    private static class ApplyItemExtensions
    {
        private static void Postfix(GearItem __instance)
        {
            if (__instance.GetComponent<AmmoItem>() != null)
            {
                _ = __instance.gameObject.GetComponent<AmmoItemExtension>() ?? __instance.gameObject.AddComponent<AmmoItemExtension>();
            }
            if (__instance.GetComponent<GunItem>() != null)
            {
                _ = __instance.gameObject.GetComponent<GunItemExtension>() ?? __instance.gameObject.AddComponent<GunItemExtension>();
            }
        }
    }

    [HarmonyPatch(typeof(GunItem), nameof(GunItem.AddRoundsToClip))]
    public static class Testing
    {
        private static void Postfix(GunItem __instance, int count, int condition)
        {
            Inventory inventory = GameManager.GetInventoryComponent();

            Il2CppSystem.Collections.Generic.List<GearItem> ammoItems = new();
            foreach (var gearItem in inventory.m_Items)
            {
                if (IsValidAmmo(gearItem, __instance.m_GearItem))
                {
                    ammoItems.Add(gearItem);
                }
            }

            Logging.Log($"Retrieved {ammoItems.Count} ammo items from inventory.");

            GunItemExtension extension = __instance.GetComponent<GunItemExtension>();
            if (extension != null)
            {
                for (int i = 0; i < count; i++)
                {
                    BulletType bulletType = FindNextBulletTypeForGun(ammoItems);
                    extension.AddRoundsToClip(bulletType, condition);
                    Logging.Log($"Added bullet to clip: Type = {bulletType}, Condition = {condition}");
                }
            }
        }

        private static BulletType FindNextBulletTypeForGun(Il2CppSystem.Collections.Generic.List<GearItem> ammoItems)
        {
            foreach (var gearItem in ammoItems)
            {
                AmmoItemExtension ammoExtension = gearItem.GetComponent<AmmoItemExtension>();
                if (ammoExtension != null)
                {
                    Logging.Log($"Valid ammo found: {gearItem.name} with BulletType = {ammoExtension.m_BulletType}");
                    return ammoExtension.m_BulletType;
                }
                else
                {
                    Logging.LogWarning($"AmmoItemExtension not found on ammo item: {gearItem.name}");
                }
            }
            Logging.LogError("No valid ammo found for gun.");
            return BulletType.Unspecified;
        }

        internal static bool IsValidAmmo(GearItem gearItem, GearItem weapon)
        {
            bool isValid = gearItem != null && gearItem.m_AmmoItem && gearItem.m_StackableItem
                           && gearItem.GetRoundedCondition() != 0
                           && gearItem.m_AmmoItem.m_AmmoForGunType == weapon.m_GunItem.m_GunType;
            Logging.Log($"Checking if gear item is valid ammo: {gearItem.name}, IsValid: {isValid}");
            return isValid;
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetAmmoAvailableForWeapon), new Type[] { typeof(GearItem) })]
    private static class TestingPatch
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
            else
            {
                Logging.LogError($"No AmmoItemExtension found on {gearItem.name}");
            }
        }
    }
}