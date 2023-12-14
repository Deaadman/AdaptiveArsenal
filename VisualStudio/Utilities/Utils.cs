namespace ExtendedWeaponry.Utilities;

internal class Utils
{
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

    internal static void UpdateAmmoCounts(GearItem gearItem, ref int apAmmoCount, ref int standardAmmoCount)
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

            Logging.Log($"AmmoItemExtension on {gearItem.name}: m_BulletType = {ammoExtension.m_BulletType}");
        }
        else
        {
            Logging.LogError($"No AmmoItemExtension found on {gearItem.name}");
        }
    }
}