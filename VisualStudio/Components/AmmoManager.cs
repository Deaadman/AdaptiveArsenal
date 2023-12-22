using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class AmmoManager : MonoBehaviour
{
    internal List<AmmoType> m_Clip = [];
    internal static AmmoType m_SelectedAmmoType;

    //private AmmoManagerSaveDataProxy m_AmmoManagerSaveDataProxy = new();
    private GunItem? m_GunItem;
    private readonly DataManager m_DataManager = new();

    private void Awake() => m_GunItem = GetComponent<GunItem>();

    internal void AddRoundsToClip(AmmoType ammoType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            m_Clip.Add(ammoType);
        }
    }

    internal static int CalculateAmmoAvailableForType()
    {
        Inventory inventory = GameManager.GetInventoryComponent();
        if (inventory == null) return 0;

        int totalAmmo = 0;
        foreach (var item in inventory.m_Items)
        {
            if (item.m_GearItem?.m_AmmoItem is AmmoItem ammoItem)
            {
                AmmoProjectile ammoProjectile = ammoItem.GetComponent<AmmoProjectile>();
                if (ammoProjectile?.m_AmmoType == m_SelectedAmmoType)
                {
                    totalAmmo += item.m_GearItem.m_StackableItem.m_Units;
                }
            }
        }

        return totalAmmo;
    }

    //internal void Deserialize()
    //{
    //    var loadedData = m_DataManager.Load();
    //    if (loadedData != null)
    //    {
    //        m_AmmoManagerSaveDataProxy = loadedData;
    //        m_Clip = m_AmmoManagerSaveDataProxy.m_Clip ?? [];
    //    }
    //}

    internal AmmoType? RemoveNextFromClip()
    {
        if (m_Clip.Count > 0)
        {
            AmmoType removedAmmo = m_Clip[0];
            m_Clip.RemoveAt(0);
            return removedAmmo;
        }

        return null;
    }

    //internal void Serialize()
    //{
    //    m_AmmoManagerSaveDataProxy.m_Clip = m_Clip;
    //    m_DataManager.Save(m_AmmoManagerSaveDataProxy);
    //}

    private static void SwitchAmmoType()
    {
        AmmoType[] ammoTypes = (AmmoType[])Enum.GetValues(typeof(AmmoType));
        int currentIndex = Array.IndexOf(ammoTypes, m_SelectedAmmoType);
        currentIndex = (currentIndex + 1) % ammoTypes.Length;
        m_SelectedAmmoType = ammoTypes[currentIndex];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchAmmoType();
        }
    }
}