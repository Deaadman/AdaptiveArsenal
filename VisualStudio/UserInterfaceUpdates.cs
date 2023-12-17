using ExtendedWeaponry.Components;
using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class UserInterfaceUpdates
{
    [HarmonyPatch(typeof(EquipItemPopup), nameof(EquipItemPopup.UpdateAmmoStatus))]
    private static class UpdateSpritesColorBasedOnAmmoType
    {
        private static void Postfix(EquipItemPopup __instance)
        {
            GunItem? gunItem = GameManager.GetPlayerManagerComponent().m_ItemInHands?.m_GunItem;
            if (gunItem == null) return;

            //if (__instance.m_LabelAmmoReserve != null)                                        // Should update this label based on which ammo is being prioritized, as right now it always defaults to AP.
            //{
            //    int ammoCount = GetPrioritizedAmmoCount(gunItem);
            //    __instance.m_LabelAmmoReserve.text = ammoCount.ToString();
            //}

            UISprite[] ammoSprites = __instance.m_ListAmmoSprites;

            if (gunItem.name.Contains("GEAR_FlareGun") && ammoSprites.Length > 0)               // Works for now, but doesn't indicate weather a bullet is loaded or not. Always indicates as if there is a bullet in the flare gun.
            {
                ammoSprites[0].color = Color.white;
            }
            else
            {
                AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
                if (ammoManager == null) return;

                for (int i = 0; i < ammoSprites.Length; i++)
                {
                    if (i < ammoManager.m_Clip.Count)
                    {
                        var bulletInfo = ammoManager.m_Clip[i];
                        Color color = AmmoManager.GetColorForBulletType(bulletInfo.m_BulletType);
                        ammoSprites[i].color = color;
                    }
                    else if (i < gunItem.m_ClipSize)
                    {
                        ammoSprites[i].color = Color.white;
                    }
                }
            }
        }
    }
}