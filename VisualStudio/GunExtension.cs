namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp]
public class GunExtension : MonoBehaviour
{
    static readonly Dictionary<string, int> GunMuzzleVelocities = new()
    {
        {"GEAR_Rifle_Barbs", 744},
        {"GEAR_Rifle_Curators", 991},
        {"GEAR_Rifle_Vaughns", 707},
        {"GEAR_Rifle", 744},
        {"GEAR_RevolverFancy", 822},
        {"GEAR_RevolverGreen", 411},
        {"GEAR_RevolverStubNosed", 275},
        {"GEAR_Revolver", 411}
    };

    internal int m_MuzzleVelocity;

    void Awake()
    {
        string gearItemGOName = GetComponent<GearItem>().gameObject.name;
        m_MuzzleVelocity = GunMuzzleVelocities.FirstOrDefault(kv => gearItemGOName.Contains(kv.Key)).Value;
    }
}