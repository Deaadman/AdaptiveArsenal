using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Patches;

internal class UIPatches
{
    [HarmonyPatch(typeof(EquipItemPopup), nameof(EquipItemPopup.UpdateAmmoStatus))]
    private static class AmmoWidgetAmmoType
    {
        private static void Postfix(EquipItemPopup __instance)
        {
            if (GameManager.GetPlayerManagerComponent().m_ItemInHands?.m_GunItem is not GunItem gunItem) return;
            if (__instance.GetComponent<EquipItemPopupAddon>() is not EquipItemPopupAddon equipItemPopupAddon) return;

            bool isFlareGun = gunItem.m_GunType == GunType.FlareGun;

            equipItemPopupAddon.m_LabelAmmoCount?.gameObject.SetActive(!isFlareGun);
            equipItemPopupAddon.m_LabelAmmoType?.gameObject.SetActive(!isFlareGun);
            __instance.m_LabelAmmoReserve.gameObject.SetActive(false);

            if (!isFlareGun && equipItemPopupAddon.m_LabelAmmoCount != null && equipItemPopupAddon.m_LabelAmmoType != null)
            {
                AmmoUtilities.UpdateAmmoSprites(gunItem, __instance.m_ListAmmoSprites);
                AmmoType currentAmmoType = AmmoManager.m_SelectedAmmoType;

                int ammoCount = AmmoManager.CalculateAmmoAvailableForType();
                equipItemPopupAddon.m_LabelAmmoCount.text = ammoCount.ToString();

                equipItemPopupAddon.m_LabelAmmoType.text = AmmoUtilities.AmmoTypeLocalization(currentAmmoType);
                equipItemPopupAddon.m_LabelAmmoType.color = AmmoUtilities.AmmoTypeColours(currentAmmoType);
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
                if (ammoAddon != null)
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
                    if (ammoAddon != null)
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