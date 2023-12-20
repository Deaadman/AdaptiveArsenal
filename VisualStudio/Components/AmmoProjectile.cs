namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class AmmoProjectile : MonoBehaviour
{
    internal AmmoType m_AmmoType;

    private void Awake()
    {
        GearItem gearItem = GetComponent<GearItem>();
        if (gearItem != null)
        {
            if (gameObject.name.Contains("GEAR_RifleAmmoSingleAP") || gameObject.name.Contains("GEAR_RifleAmmoBoxAP"))
            {
                m_AmmoType = AmmoType.ArmorPiercing;
            }
            else if (gameObject.name.Contains("GEAR_RevolverAmmoSingle") || gameObject.name.Contains("GEAR_RifleAmmoSingle") || gameObject.name.Contains("GEAR_RevolverAmmoBox") || gameObject.name.Contains("GEAR_RifleAmmoBox"))
            {
                m_AmmoType = AmmoType.FullMetalJacket;
            }
        }
    }
}