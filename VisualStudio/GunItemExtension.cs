using ExtendedWeaponry.Utilities;
using Il2CppInterop.Runtime.Attributes;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp]
public class GunItemExtension(IntPtr intPtr) : MonoBehaviour(intPtr)
{
    internal struct BulletInfo
    {
        public BulletType m_BulletType;

        public int m_Condition;
    }

    private List<BulletInfo> m_Clip = [];

    internal void AddRoundsToClip(BulletType bulletType, int condition)
    {
        GunItem gunItem = GetComponent<GunItem>();
        if (gunItem != null && m_Clip.Count < gunItem.m_ClipSize)
        {
            m_Clip.Add(new BulletInfo { m_BulletType = bulletType, m_Condition = condition });
        }
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