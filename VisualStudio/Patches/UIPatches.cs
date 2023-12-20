using ExtendedWeaponry.Components;
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
            if (gunItem?.GetComponent<AmmoManager>() is not AmmoManager ammoManager) return;

            AmmoUtilities.UpdateAmmoSprites(gunItem, __instance.m_ListAmmoSprites);

            if (__instance.GetComponent<EquipItemPopupAddon>() is not EquipItemPopupAddon equipItemPopupExtension) return;

            if (ammoWidgetClones.Count == 0)
            {
                foreach (AmmoType bulletType in Enum.GetValues(typeof(AmmoType)))
                {
                    __instance.m_LabelAmmoReserve.gameObject.SetActive(bulletType == AmmoType.Unspecified);
                    if (bulletType == AmmoType.Unspecified || equipItemPopupExtension.m_AmmoWidgetExtensionPrefab is not GameObject prefab) continue;

                    GameObject prefabInstance = UnityEngine.Object.Instantiate(prefab, prefab.transform.parent);
                    prefabInstance.name = $"AmmoWidgetExtension_{bulletType}";

                    if (prefabInstance.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>() is UILabel labelAmmoCount &&
                        prefabInstance.transform.Find("Label_AmmoType")?.GetComponent<UILabel>() is UILabel labelAmmoType)
                    {
                        labelAmmoCount.text = AmmoUtilities.GetAmmoCountInInventory(bulletType, gunItem).ToString();
                        labelAmmoType.text = bulletType.ToString();
                        ammoWidgetClones[bulletType] = prefabInstance;
                    }
                }
            }

            AmmoType prioritizedBulletType = ammoManager.GetNextAmmoType();

            foreach (var pair in ammoWidgetClones)
            {
                int ammoCount = AmmoUtilities.GetAmmoCountInInventory(pair.Key, gunItem);
                if (pair.Value.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>() is UILabel labelAmmoCount)
                {
                    labelAmmoCount.text = ammoCount.ToString();
                }

                if (pair.Value.transform.Find("Label_AmmoType")?.GetComponent<UILabel>() is UILabel labelAmmoType)
                {
                    labelAmmoType.text = AmmoUtilities.AmmoTypeLocalization(pair.Key);
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
            __instance.m_ItemNotesLabel.gameObject.SetActive(false);

            if (gi.m_AmmoItem)
            {
                AmmoProjectile ammoAddon = gi.GetComponent<AmmoProjectile>();
                if (ammoAddon != null && ammoAddon.m_AmmoType != AmmoType.Unspecified)
                {
                    __instance.m_ItemNotesLabel.gameObject.SetActive(true);
                    __instance.m_ItemNotesLabel.text = AmmoUtilities.AmmoTypeLocalization(ammoAddon.m_AmmoType);
                    __instance.m_ItemNotesLabel.color = AmmoUtilities.AmmoTypeColours(ammoAddon.m_AmmoType);
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

            if (hudAddon.m_AmmoTypePrefab != null)
            {
                hudAddon.m_AmmoTypePrefab.SetActive(false);

                if (__instance.m_Gear.m_AmmoItem)
                {
                    AmmoProjectile ammoAddon = __instance.m_Gear.m_AmmoItem.GetComponent<AmmoProjectile>();
                    if (ammoAddon != null && ammoAddon.m_AmmoType != AmmoType.Unspecified)
                    {
                        if (hudAddon.m_LabelInspectAmmoType != null)
                        {
                            hudAddon.m_LabelInspectAmmoType.text = AmmoUtilities.AmmoTypeLocalization(ammoAddon.m_AmmoType);
                            hudAddon.m_LabelInspectAmmoType.color = AmmoUtilities.AmmoTypeColours(ammoAddon.m_AmmoType);
                            hudAddon.m_AmmoTypePrefab.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}