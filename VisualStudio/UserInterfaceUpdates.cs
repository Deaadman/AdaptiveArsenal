using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class UserInterfaceUpdates
{
    [HarmonyPatch(typeof(EquipItemPopup), nameof(EquipItemPopup.UpdateAmmoStatus))]
    private static class UpdateUIBasedOnAmmoType
    {
        private static readonly Dictionary<BulletType, GameObject> ammoWidgetClones = [];

        private static void Postfix(EquipItemPopup __instance)
        {
            GunItem? gunItem = GameManager.GetPlayerManagerComponent().m_ItemInHands?.m_GunItem;
            if (gunItem == null) return;

            AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            UISprite[] ammoSprites = __instance.m_ListAmmoSprites;
            AmmoManager.UpdateAmmoSprites(gunItem, ammoSprites);

            BulletType prioritizedBulletType = ammoManager.GetNextBulletType();

            EquipItemPopupExtension equipItemPopupExtension = __instance.GetComponent<EquipItemPopupExtension>();
            if (equipItemPopupExtension == null) return;

            if (ammoWidgetClones.Count == 0)
            {
                foreach (BulletType bulletType in Enum.GetValues(typeof(BulletType)))
                {
                    if (bulletType == BulletType.Unspecified)
                    {
                        __instance.m_LabelAmmoReserve.gameObject.SetActive(true);
                        continue;
                    }
                    else
                    {
                        GameObject? prefab = equipItemPopupExtension.m_AmmoWidgetExtensionPrefab;
                        if (prefab == null) continue;

                        GameObject prefabInstance = UnityEngine.Object.Instantiate(prefab, prefab.transform.parent);
                        prefabInstance.name = $"AmmoWidgetExtension_{bulletType}";

                        UILabel? labelAmmoCount = prefabInstance.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>();
                        UILabel? labelAmmoType = prefabInstance.transform.Find("Label_AmmoType")?.GetComponent<UILabel>();
                        if (labelAmmoCount == null || labelAmmoType == null) continue;

                        labelAmmoCount.text = AmmoManager.GetBulletCountInInventory(bulletType, gunItem).ToString();
                        labelAmmoType.text = bulletType.ToString();

                        ammoWidgetClones[bulletType] = prefabInstance;

                        __instance.m_LabelAmmoReserve.gameObject.SetActive(false);
                    }
                }
            }

            foreach (var pair in ammoWidgetClones)
            {
                int ammoCount = AmmoManager.GetBulletCountInInventory(pair.Key, gunItem);
                UILabel? labelAmmoCount = pair.Value.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>();
                UILabel? labelAmmoType = pair.Value.transform.Find("Label_AmmoType")?.GetComponent<UILabel>();

                if (labelAmmoCount != null)
                {
                    labelAmmoCount.text = ammoCount.ToString();
                }

                if (labelAmmoType != null)
                {
                    string bulletTypeName = AmmoManager.FormatBulletTypeName(pair.Key);
                    labelAmmoType.text = $"{bulletTypeName}";
                    labelAmmoType.color = AmmoManager.GetColorForBulletType(pair.Key);
                }

                pair.Value.SetActive(pair.Key == prioritizedBulletType);
            }
        }
    }

    [HarmonyPatch(typeof(ItemDescriptionPage), nameof(ItemDescriptionPage.UpdateGearItemDescription))]
    private static class InventoryAmmoType
    {
        private static void Postfix(ItemDescriptionPage __instance, GearItem gi)
        {
            if (gi.m_AmmoItem)
            {
                AmmoExtension ammoExtension = gi.GetComponent<AmmoExtension>();
                if (ammoExtension != null)
                {
                    BulletType bulletType = ammoExtension.m_BulletType;
                    __instance.m_ItemNotesLabel.gameObject.SetActive(true);
                    __instance.m_ItemNotesLabel.text = AmmoManager.FormatBulletTypeName(bulletType);
                    __instance.m_ItemNotesLabel.color = AmmoManager.GetColorForBulletType(bulletType);
                }
                else
                {
                    __instance.m_ItemNotesLabel.gameObject.SetActive(false);
                }
            }
        }
    }
}