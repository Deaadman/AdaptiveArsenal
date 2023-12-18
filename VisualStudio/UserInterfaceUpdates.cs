using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class UserInterfaceUpdates
{
    [HarmonyPatch(typeof(EquipItemPopup), nameof(EquipItemPopup.UpdateAmmoStatus))]
    private static class UpdateUIBasedOnAmmoType
    {
        private static UILabel clonedLabel = null;

        private static void Postfix(EquipItemPopup __instance)
        {
            GunItem? gunItem = GameManager.GetPlayerManagerComponent().m_ItemInHands?.m_GunItem;
            if (gunItem == null) return;

            UISprite[] ammoSprites = __instance.m_ListAmmoSprites;
            AmmoManager.UpdateAmmoSprites(gunItem, ammoSprites);

            if (clonedLabel == null)
            {
                clonedLabel = UnityEngine.Object.Instantiate(__instance.m_LabelAmmoReserve, __instance.m_LabelAmmoReserve.transform.parent);
                clonedLabel.gameObject.name = "ClonedAmmoReserveLabel";
                clonedLabel.transform.localPosition = new Vector3(__instance.m_LabelAmmoReserve.transform.localPosition.x,
                                                                 __instance.m_LabelAmmoReserve.transform.localPosition.y - 20,
                                                                 __instance.m_LabelAmmoReserve.transform.localPosition.z);
            }

            int standardAmmoCount = GetBulletCountInInventory(BulletType.Standard, gunItem);
            __instance.m_LabelAmmoReserve.text = $"ST: {standardAmmoCount}";

            int armorPiercingAmmoCount = GetBulletCountInInventory(BulletType.ArmorPiercing, gunItem);
            clonedLabel.text = $"AP: {armorPiercingAmmoCount}";
        }

        private static int GetBulletCountInInventory(BulletType bulletType, GunItem gunItem)
        {
            Inventory inventory = GameManager.GetInventoryComponent();
            if (inventory == null)
            {
                Logging.LogError("Inventory component not found.");
                return 0;
            }

            int count = 0;
            foreach (var gearItemObject in inventory.m_Items)
            {
                GearItem gearItem = gearItemObject;
                if (gearItem != null && AmmoManager.IsValidAmmo(gearItem, gunItem.m_GearItem))
                {
                    AmmoItemExtension ammoExtension = gearItem.gameObject.GetComponent<AmmoItemExtension>();
                    if (ammoExtension != null && ammoExtension.m_BulletType == bulletType)
                    {
                        count += gearItem.m_StackableItem.m_Units;
                    }
                }
            }

            return count;
        }
    }
}