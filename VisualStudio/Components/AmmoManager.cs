using ExtendedWeaponry.Utilities;
using Il2CppInterop.Runtime.Attributes;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class AmmoManager : MonoBehaviour
{
    internal List<AmmoType> m_Clip = [];

    private AmmoManagerSaveDataProxy m_AmmoManagerSaveDataProxy = new();
    private GunItem? m_GunItem;
    private SaveDataManager m_SaveDataManager = new();

    private void Awake()
    {
        m_GunItem = GetComponent<GunItem>();
    }

    internal void AddRoundsToClip(AmmoType bulletType)
    {
        if (m_GunItem != null && m_Clip.Count < m_GunItem.m_ClipSize)
        {
            m_Clip.Add(bulletType);
        }
    }

    internal void Deserialize()
    {
        var loadedData = m_SaveDataManager.Load();
        if (loadedData != null)
        {
            m_AmmoManagerSaveDataProxy = loadedData;
            m_Clip = m_AmmoManagerSaveDataProxy.m_Clip ?? [];
        }
    }

    internal int GetLoadedAmmoCount()
    {
        return m_Clip.Count;
    }

    internal AmmoType GetNextAmmoType()
    {
        Inventory inventory = GameManager.GetInventoryComponent();
        if (inventory == null)
        {
            Logging.LogError("Inventory component not found.");
            return AmmoType.Unspecified;
        }

        if (m_GunItem == null)
        {
            Logging.LogError("GunItem component not found on AmmoManager.");
            return AmmoType.Unspecified;
        }

        Il2CppSystem.Collections.Generic.List<GearItem> ammoItems = new();
        foreach (var gearItem in inventory.m_Items)
        {
            if (AmmoUtilities.IsValidAmmoType(gearItem, m_GunItem.m_GearItem))
            {
                ammoItems.Add(gearItem);
            }
        }

        foreach (var gearItem in ammoItems)
        {
            AmmoProjectile ammoExtension = gearItem.gameObject.GetComponent<AmmoProjectile>();
            if (ammoExtension != null)
            {
                return ammoExtension.m_AmmoType;
            }
        }

        return AmmoType.Unspecified;
    } 

    [HideFromIl2Cpp]
    internal bool RemoveAmmoFromClip(out AmmoType bulletType)
    {
        if (m_Clip.Count > 0)
        {
            bulletType = m_Clip[0];
            m_Clip.RemoveAt(0);
            return true;
        }
        else
        {
            bulletType = AmmoType.Unspecified;
            return false;
        }
    }

    internal void Serialize()
    {
        m_AmmoManagerSaveDataProxy.m_Clip = m_Clip;
        m_SaveDataManager.Save(m_AmmoManagerSaveDataProxy);
    }
}