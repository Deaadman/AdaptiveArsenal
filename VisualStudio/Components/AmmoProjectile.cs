namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class AmmoProjectile : MonoBehaviour
{
    internal AmmoType m_AmmoType;

    private void Awake()
    {
        AmmoItem ammoItem = GetComponent<AmmoItem>();
        if (ammoItem != null)
        {
            if (gameObject.name.StartsWith("GEAR_RifleAmmoSingleAP") || gameObject.name.StartsWith("GEAR_RifleAmmoBoxAP"))
            {
                m_AmmoType = AmmoType.ArmorPiercing;
            }
            else if (gameObject.name.StartsWith("GEAR_RevolverAmmoSingle") || gameObject.name.StartsWith("GEAR_RifleAmmoSingle") || gameObject.name.StartsWith("GEAR_RevolverAmmoBox") || gameObject.name.StartsWith("GEAR_RifleAmmoBox"))
            {
                m_AmmoType = AmmoType.FullMetalJacket;
            }
        }
    }
}