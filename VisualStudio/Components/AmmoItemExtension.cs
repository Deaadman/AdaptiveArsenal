using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class AmmoItemExtension : MonoBehaviour
{
    internal BulletType m_BulletType;

    private void Awake()
    {
        GearItem gearItem = GetComponent<GearItem>();
        if (gearItem != null)
        {
            if (gameObject.name.Contains("GEAR_RifleAmmoSingleAP") || gameObject.name.Contains("GEAR_RifleAmmoBoxAP"))
            {
                m_BulletType = BulletType.ArmorPiercing;
            }
            else if (gameObject.name.Contains("GEAR_RevolverAmmoSingle") || gameObject.name.Contains("GEAR_RifleAmmoSingle") || gameObject.name.Contains("GEAR_RevolverAmmoBox") || gameObject.name.Contains("GEAR_RifleAmmoBox"))
            {
                m_BulletType = BulletType.Standard;
            }
        }
    }
}