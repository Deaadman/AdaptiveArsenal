namespace AdaptiveArsenal;

[RegisterTypeInIl2Cpp(false)]
public class GunExtension : MonoBehaviour
{
    static readonly Dictionary<string, GunStats> m_GunData = new()
    {
        {"GEAR_Rifle_Barbs", new GunStats(744, 175, 325)},
        {"GEAR_Rifle_Curators", new GunStats(991, 200, 400)},
        {"GEAR_Rifle_Vaughns", new GunStats(707, 150, 300)},
        {"GEAR_Rifle", new GunStats(744, 150, 300)},
        {"GEAR_RevolverFancy", new GunStats(822, 100, 200)},
        {"GEAR_RevolverGreen", new GunStats(411, 75, 150)},
        {"GEAR_RevolverStubNosed", new GunStats(275, 50, 100)},
        {"GEAR_Revolver", new GunStats(411, 75, 150)}
    };

    internal class GunStats(int muzzleVelocity, int effectiveRange, int maxRange)
    {
        internal int m_MuzzleVelocity = muzzleVelocity;
        internal int m_EffectiveRange = effectiveRange;
        internal int m_MaxRange = maxRange;
    }

#nullable disable
    internal GunStats m_GunStats;
#nullable enable

    void Awake()
    {
        string gearItemGOName = GetComponent<GearItem>().gameObject.name;
        m_GunStats = m_GunData.FirstOrDefault(kv => gearItemGOName.Contains(kv.Key)).Value;
    }
}