using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp]
public class GunItemExtension(IntPtr intPtr) : MonoBehaviour(intPtr)
{
    public struct BulletInfo
    {
        public BulletType m_BulletType;

        public int m_Condition;
    }

    private List<BulletInfo> m_Clip = [];

    public void AddRoundsToClip(BulletType bulletType, int condition)
    {
        GunItem gunItem = GetComponent<GunItem>();
        if (gunItem != null && m_Clip.Count < gunItem.m_ClipSize)
        {
            m_Clip.Add(new BulletInfo { m_BulletType = bulletType, m_Condition = condition });
        }
    }
}