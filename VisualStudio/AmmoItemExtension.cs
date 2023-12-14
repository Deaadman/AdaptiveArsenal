using ExtendedWeaponry.Utilities;
using ModComponent.API.Components;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp]
public class AmmoItemExtension(IntPtr intPtr) : MonoBehaviour(intPtr)
{
    public BulletType m_BulletType;

    private void Awake()
    {
        AmmoItem ammoItem = GetComponent<AmmoItem>();
        if (ammoItem != null)
        {
            if (gameObject.name.Contains("GEAR_RevolverAmmoSingle") || gameObject.name.Contains("GEAR_RifleAmmoSingle") || gameObject.name.Contains("GEAR_RevolverAmmoBox") || gameObject.name.Contains("GEAR_RifleAmmoBox"))
            {
                m_BulletType = BulletType.Standard;
            }
        }

        ModAmmoComponent modAmmoComponent = GetComponent<ModAmmoComponent>();
        if (modAmmoComponent != null)
        {
            if (gameObject.name.Contains("GEAR_RifleAmmoSingleAP") || gameObject.name.Contains("GEAR_RifleAmmoBoxAP"))
            {
                m_BulletType = BulletType.ArmorPiercing;
            }
        }
    }
}