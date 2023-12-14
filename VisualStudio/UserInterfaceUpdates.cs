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

            AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            UISprite[] ammoSprites = __instance.m_ListAmmoSprites;

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