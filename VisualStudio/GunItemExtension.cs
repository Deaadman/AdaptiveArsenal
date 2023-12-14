namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp]
public class GunItemExtension(IntPtr intPtr) : MonoBehaviour(intPtr)
{
    public List<GearItem> m_AmmoItemPrefabs;

    public string SelectedAmmoPrefabName { get; set; }

    private void Awake()
    {
        GunItem gunItem = GetComponent<GunItem>();
        if (gunItem != null)
        {
            GearItem[] ammoItemArray = GetCompatibleAmmoPrefabs(gunItem.m_GunType);
            m_AmmoItemPrefabs = ConvertArrayToList(ammoItemArray);
        }
    }

    private static GearItem[] GetCompatibleAmmoPrefabs(GunType gunType)
    {
        return gunType switch
        {
            GunType.Rifle =>
            [
            GearItem.LoadGearItemPrefab("GEAR_RifleAmmoSingle"),
            GearItem.LoadGearItemPrefab("GEAR_RifleAmmoBox"),
            GearItem.LoadGearItemPrefab("GEAR_RifleAmmoSingleAP"),
            GearItem.LoadGearItemPrefab("GEAR_RifleAmmoBoxAP")
            ],
            GunType.Revolver =>
            [
            GearItem.LoadGearItemPrefab("GEAR_RevolverAmmoSingle"),
            GearItem.LoadGearItemPrefab("GEAR_RevolverAmmoBox")
            ],
            _ => []
        };
    }

    private static List<GearItem> ConvertArrayToList(GearItem[] gearItems)
    {
        return new List<GearItem>(gearItems);
    }
}