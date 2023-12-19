﻿using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Patches;

internal class UIPatches
{
    [HarmonyPatch(typeof(EquipItemPopup), nameof(EquipItemPopup.UpdateAmmoStatus))]
    private static class AmmoWidgetAmmoType
    {
        private static readonly Dictionary<AmmoType, GameObject> ammoWidgetClones = [];

        private static void Postfix(EquipItemPopup __instance)
        {
            GunItem? gunItem = GameManager.GetPlayerManagerComponent().m_ItemInHands?.m_GunItem;
            if (gunItem == null) return;

            AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            UISprite[] ammoSprites = __instance.m_ListAmmoSprites;
            AmmoUtilities.UpdateAmmoSprites(gunItem, ammoSprites);

            AmmoType prioritizedBulletType = ammoManager.GetNextAmmoType();

            EquipItemPopupAddon equipItemPopupExtension = __instance.GetComponent<EquipItemPopupAddon>();
            if (equipItemPopupExtension == null) return;

            if (ammoWidgetClones.Count == 0)
            {
                foreach (AmmoType bulletType in Enum.GetValues(typeof(AmmoType)))
                {
                    if (bulletType == AmmoType.Unspecified)
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

                        labelAmmoCount.text = AmmoUtilities.GetAmmoCountInInventory(bulletType, gunItem).ToString();
                        labelAmmoType.text = bulletType.ToString();

                        ammoWidgetClones[bulletType] = prefabInstance;

                        __instance.m_LabelAmmoReserve.gameObject.SetActive(false);
                    }
                }
            }

            foreach (var pair in ammoWidgetClones)
            {
                int ammoCount = AmmoUtilities.GetAmmoCountInInventory(pair.Key, gunItem);
                UILabel? labelAmmoCount = pair.Value.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>();
                UILabel? labelAmmoType = pair.Value.transform.Find("Label_AmmoType")?.GetComponent<UILabel>();

                if (labelAmmoCount != null)
                {
                    labelAmmoCount.text = ammoCount.ToString();
                }

                if (labelAmmoType != null)
                {
                    string bulletTypeName = AmmoUtilities.AmmoTypeLocalization(pair.Key);
                    labelAmmoType.text = $"{bulletTypeName}";
                    labelAmmoType.color = AmmoUtilities.AmmoTypeColours(pair.Key);
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
                AmmoAddon ammoAddon = gi.GetComponent<AmmoAddon>();
                if (ammoAddon != null)
                {
                    AmmoType ammoType = ammoAddon.m_AmmoType;
                    __instance.m_ItemNotesLabel.gameObject.SetActive(true);
                    __instance.m_ItemNotesLabel.text = AmmoUtilities.AmmoTypeLocalization(ammoType);
                    __instance.m_ItemNotesLabel.color = AmmoUtilities.AmmoTypeColours(ammoType);
                }
                else
                {
                    __instance.m_ItemNotesLabel.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.InitLabelsForGear))]
    private static class InspectAmmoType
    {
        private static void Prefix(PlayerManager __instance)
        {
            Panel_HUD panelHUD = InterfaceManager.GetPanel<Panel_HUD>();
            PanelHUDAddon hudAddon = panelHUD.GetComponent<PanelHUDAddon>();

            if (hudAddon.m_Prefab != null)
            {
                if (__instance.m_Gear.m_AmmoItem)
                {
                    AmmoAddon ammoAddon = __instance.m_Gear.m_AmmoItem.GetComponent<AmmoAddon>();
                    if (ammoAddon != null && hudAddon.m_LabelInspectAmmoType != null)
                    {
                        AmmoType ammoType = ammoAddon.m_AmmoType;
                        hudAddon.m_LabelInspectAmmoType.text = AmmoUtilities.AmmoTypeLocalization(ammoType);
                        hudAddon.m_LabelInspectAmmoType.color = AmmoUtilities.AmmoTypeColours(ammoType);
                        hudAddon.m_Prefab.SetActive(true);
                    }
                }
                else
                {
                    hudAddon.m_Prefab.SetActive(false);
                }
            }
        }
    }
}