using ExtendedWeaponry.Utilities;
using Il2CppInterop.Runtime.Attributes;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp(false)]
public class AmmoManager : MonoBehaviour
{
    internal struct BulletInfo
    {
        public BulletType m_BulletType;
        public int m_Condition;
    }

    internal List<BulletInfo> m_Clip = [];

    internal void AddRoundsToClip(BulletType bulletType, int condition)
    {
        GunItem gunItem = GetComponent<GunItem>();
        if (gunItem != null && m_Clip.Count < gunItem.m_ClipSize)
        {
            m_Clip.Add(new BulletInfo { m_BulletType = bulletType, m_Condition = condition });
        }
    }

    internal BulletType FindNextBulletTypeForGun(Il2CppSystem.Collections.Generic.List<GearItem> ammoItems)
    {
        foreach (var gearItem in ammoItems)
        {
            AmmoItemExtension ammoExtension = gearItem.GetComponent<AmmoItemExtension>();
            if (ammoExtension != null)
            {
                Logging.Log($"Valid ammo found: {gearItem.name} with BulletType = {ammoExtension.m_BulletType}");
                return ammoExtension.m_BulletType;
            }
        }
        Logging.LogError("No valid ammo found for gun.");
        return BulletType.Unspecified;
    }

    internal static Color GetColorForBulletType(BulletType bulletType)
    {
        return bulletType switch
        {
            BulletType.ArmorPiercing => Color.green,
            BulletType.Standard => Color.yellow,
            _ => Color.white,
        };
    }

    internal bool IsValidAmmo(GearItem gearItem, GearItem weapon)
    {
        bool isValid = gearItem != null && gearItem.m_AmmoItem && gearItem.m_StackableItem && gearItem.GetRoundedCondition() != 0 && gearItem.m_AmmoItem.m_AmmoForGunType == weapon.m_GunItem.m_GunType;
        return isValid;
    }

    [HideFromIl2Cpp]
    internal bool RemoveNextFromClip(out BulletInfo bulletInfo)
    {
        if (m_Clip.Count > 0)
        {
            bulletInfo = m_Clip[0];
            m_Clip.RemoveAt(0);
            return true;
        }
        else
        {
            bulletInfo = new BulletInfo();
            return false;
        }
    }
}