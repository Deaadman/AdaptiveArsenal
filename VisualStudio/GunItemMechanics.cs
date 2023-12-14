using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class GunItemMechanics
{
    [HarmonyPatch(typeof(GunItem), nameof(GunItem.AddRoundsToClip))]
    private static class ApplyDataToCustomClip
    {
        private static void Postfix(GunItem __instance, int count, int condition)
        {
            Inventory inventory = GameManager.GetInventoryComponent();

            Il2CppSystem.Collections.Generic.List<GearItem> ammoItems = new();
            AmmoManager extension = __instance.GetComponent<AmmoManager>();
            if (extension == null) return;

            foreach (var gearItem in inventory.m_Items)
            {
                if (extension.IsValidAmmo(gearItem, __instance.m_GearItem))
                {
                    ammoItems.Add(gearItem);
                }
            }

            Logging.Log($"Retrieved {ammoItems.Count} ammo items from inventory.");

            for (int i = 0; i < count; i++)
            {
                BulletType bulletType = extension.FindNextBulletTypeForGun(ammoItems);
                extension.AddRoundsToClip(bulletType, condition);
                Logging.Log($"Added bullet to clip: Type = {bulletType}, Condition = {condition}");
            }
        }
    }

    [HarmonyPatch(typeof(GunItem), nameof(GunItem.RemoveNextFromClip))]
    private static class RemoveDataFromCustomClip
    {
        private static void Prefix(GunItem __instance)
        {
            AmmoManager extension = __instance.GetComponent<AmmoManager>();
            if (extension != null)
            {
                if (extension.RemoveNextFromClip(out AmmoManager.BulletInfo bulletInfo))
                {
                    Logging.Log($"Removed bullet from clip: BulletType = {bulletInfo.m_BulletType}");
                }
            }
        }
    }
}