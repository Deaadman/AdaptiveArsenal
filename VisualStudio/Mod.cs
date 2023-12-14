using ExtendedWeaponry.Utilities;
using ModComponent.API.Components;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
    private static class ApplyBulletTypes
    {
        private static void Postfix(GearItem __instance)
        {
            if (__instance.GetComponent<AmmoItem>() != null || __instance.GetComponent<ModAmmoComponent>() != null)
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
    private static class AddRoundsToClipPatch
    {
        private static void Postfix(GunItem __instance, int count)
        {
            GunItemExtension gunItemExtension = __instance.GetComponent<GunItemExtension>();

            if (gunItemExtension == null)
            {
                Logging.LogError("GunItemExtension not found on GunItem.");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                int clipIndex = __instance.m_Clip.Count - 1 - i;

                AmmoItem ammoItem = GetAmmoItemForRound(__instance, gunItemExtension, i);
                if (ammoItem != null)
                {
                    AmmoItemExtension ammoExtension = ammoItem.GetComponent<AmmoItemExtension>();
                    if (ammoExtension != null)
                    {
                        BulletType bulletType = ammoExtension.m_BulletType;
                        AmmoManager.SetAmmoType(clipIndex, bulletType);
                        Logging.Log($"Added {bulletType} ammo to clip at index {clipIndex}.");
                    }
                }
            }
        }
    }

    private static AmmoItem GetAmmoItemForRound(GunItem gunItem, GunItemExtension gunItemExtension, int roundIndex)
    {
        if (gunItem != null && roundIndex >= 0 && roundIndex < gunItem.m_Clip.Count)
        {
            string selectedAmmoPrefabName = gunItemExtension.SelectedAmmoPrefabName;

            var selectedAmmoPrefab = gunItemExtension.m_AmmoItemPrefabs.FirstOrDefault(p => p.name == selectedAmmoPrefabName);
            if (selectedAmmoPrefab != null)
            {
                var ammoItem = selectedAmmoPrefab.GetComponent<AmmoItem>();
                if (ammoItem != null)
                {
                    Logging.Log($"Selected ammo prefab: {selectedAmmoPrefab.name} for round index {roundIndex}");
                    var ammoExtension = ammoItem.GetComponent<AmmoItemExtension>();
                    if (ammoExtension != null)
                    {
                        return ammoItem;
                    }
                    else
                    {
                        Logging.Log($"AmmoItemExtension component not found on GearItem prefab for round at index {roundIndex}.");
                    }
                }
                else
                {
                    Logging.Log($"AmmoItem component not found on GearItem prefab for round at index {roundIndex}.");
                }
            }
            else
            {
                Logging.Log($"Selected ammo prefab not found in m_AmmoItemPrefabs for round at index {roundIndex}.");
            }
        }
        return null;
    }

    [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
    private static class Testing2
    {
        private static void Prefix(GunItem __instance)
        {
            try
            {
                int clipIndex = __instance.m_Clip.Count - 1;
                BulletType bulletType = AmmoManager.GetAmmoType(clipIndex);
                Logging.Log($"Fired round with {bulletType} ammo from clip index {clipIndex}.");
            }
            catch (Exception ex)
            {
                Logging.LogError($"Error in FiredPatch Prefix: {ex.Message}");
            }
        }
    }
}