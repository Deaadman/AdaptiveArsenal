using ExtendedWeaponry.Utilities;
using Il2CppInterop.Runtime.Attributes;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class AmmoManager : MonoBehaviour
{
    internal List<BulletType> m_Clip = [];
    private GunItem? m_GunItem;
    private static AmmoManagerSaveDataProxy m_AmmoManagerSaveDataProxy = new();

    private void Awake()
    {
        m_GunItem = GetComponent<GunItem>();
    }

    internal void AddRoundsToClip(BulletType bulletType)
    {
        if (m_GunItem != null && m_Clip.Count < m_GunItem.m_ClipSize)
        {
            m_Clip.Add(bulletType);
        }
    }

    internal static string FormatBulletTypeName(BulletType bulletType)
    {
        return bulletType switch
        {
            BulletType.ArmorPiercing => Localization.Get("GAMEPLAY_ArmorPiercing"),
            BulletType.FullMetalJacket => Localization.Get("GAMEPLAY_FullMetalJacket"),
            _ => bulletType.ToString(),
        };
    }

    internal static int GetBulletCountInInventory(BulletType bulletType, GunItem gunItem)
    {
        Inventory inventory = GameManager.GetInventoryComponent();
        if (inventory == null)
        {
            Logging.LogError("Inventory component not found.");
            return 0;
        }

        int count = 0;
        foreach (var gearItemObject in inventory.m_Items)
        {
            GearItem gearItem = gearItemObject;
            if (gearItem != null && IsValidAmmo(gearItem, gunItem.m_GearItem))
            {
                AmmoExtension ammoExtension = gearItem.gameObject.GetComponent<AmmoExtension>();
                if (ammoExtension != null && ammoExtension.m_BulletType == bulletType)
                {
                    count += gearItem.m_StackableItem.m_Units;
                }
            }
        }

        return count;
    }

    internal static Color GetColorForBulletType(BulletType bulletType)
    {
        return bulletType switch
        {
            BulletType.ArmorPiercing => Color.green,
            BulletType.FullMetalJacket => Color.yellow,
            BulletType.Unspecified => Color.white,                          // Could use this for the Flare Gun items instead, would just need a way to get the game to run reload normally if the BulletType is Unspecified.
            _ => Color.white,
        };
    }

    internal int GetLoadedBulletCount()
    {
        return m_Clip.Count;
    }

    internal static Material? GetMaterialForBulletType(BulletType bulletType)
    {
        string? prefabName = GetPrefabNameForBulletType(bulletType);
        if (prefabName == null) return null;

        Material[]? materials = TextureSwapper.GetMaterialsFromGearItemPrefab(prefabName);
        if (materials != null && materials.Length > 0)
        {
            return materials[0];
        }
        else
        {
            return null;
        }
    }

    internal BulletType GetNextBulletType()
    {
        Inventory inventory = GameManager.GetInventoryComponent();
        if (inventory == null)
        {
            Logging.LogError("Inventory component not found.");
            return BulletType.Unspecified;
        }

        if (m_GunItem == null)
        {
            Logging.LogError("GunItem component not found on AmmoManager.");
            return BulletType.Unspecified;
        }

        Il2CppSystem.Collections.Generic.List<GearItem> ammoItems = new();
        foreach (var gearItem in inventory.m_Items)
        {
            if (IsValidAmmo(gearItem, m_GunItem.m_GearItem))
            {
                ammoItems.Add(gearItem);
            }
        }

        foreach (var gearItem in ammoItems)
        {
            AmmoExtension ammoExtension = gearItem.gameObject.GetComponent<AmmoExtension>();
            if (ammoExtension != null)
            {
                return ammoExtension.m_BulletType;
            }
        }

        return BulletType.Unspecified;
    }

    private static string? GetPrefabNameForBulletType(BulletType bulletType)
    {
        return bulletType switch
        {
            BulletType.ArmorPiercing => "GEAR_RifleAmmoSingleAP",
            BulletType.FullMetalJacket => "GEAR_RifleAmmoSingle",
            _ => null,
        };
    }

    internal static bool IsValidAmmo(GearItem gearItem, GearItem weapon)
    {
        if (gearItem == null || weapon == null || gearItem.m_AmmoItem == null || weapon.m_GunItem == null) return false;

        bool isValid = gearItem.m_AmmoItem && gearItem.m_StackableItem && gearItem.GetRoundedCondition() != 0 && gearItem.m_AmmoItem.m_AmmoForGunType == weapon.m_GunItem.m_GunType;
        return isValid;
    }

    [HideFromIl2Cpp]
    internal static void PrioritizeBulletType(GearItem gearItem, Dictionary<BulletType, int> bulletTypeCounts)
    {
        AmmoExtension ammoExtension = gearItem.GetComponent<AmmoExtension>();
        if (ammoExtension != null)
        {
            BulletType bulletType = ammoExtension.m_BulletType;
            if (!bulletTypeCounts.ContainsKey(bulletType))
            {
                bulletTypeCounts[bulletType] = 0;
            }
            bulletTypeCounts[bulletType] += gearItem.m_StackableItem.m_Units;
        }
    }

    [HideFromIl2Cpp]
    internal bool RemoveNextFromClip(out BulletType bulletType)
    {
        if (m_Clip.Count > 0)
        {
            bulletType = m_Clip[0];
            m_Clip.RemoveAt(0);
            return true;
        }
        else
        {
            bulletType = BulletType.Unspecified;
            return false;
        }
    }

    internal static void UpdateAmmoSprites(GunItem gunItem, UISprite[] ammoSprites)         // There is the issue again with the flare gun ammo sprite copying the last colour of the sprite loaded from other weapons.
    {
        if (gunItem.name.Contains("GEAR_FlareGun") && ammoSprites.Length > 0)
        {
            ammoSprites[0].color = Color.white;
        }
        else
        {
            AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            for (int i = 0; i < ammoSprites.Length; i++)
            {
                if (i < ammoManager.m_Clip.Count)
                {
                    BulletType bulletType = ammoManager.m_Clip[i];
                    Color color = GetColorForBulletType(bulletType);
                    ammoSprites[i].color = color;
                }
                else if (i < gunItem.m_ClipSize)
                {
                    ammoSprites[i].color = Color.white;
                }
            }
        }
    }

    internal static void UpdateBulletMaterials(Transform meshesTransform, AmmoManager ammoManager, Material nextBulletMaterial)
    {
        int loadedBullets = ammoManager.GetLoadedBulletCount();
        string bulletMeshName = loadedBullets == 1 ? "mesh_bullet_a" : "mesh_bullet_b";
        TextureSwapper.SwapMaterial(meshesTransform, bulletMeshName, nextBulletMaterial);
        TextureSwapper.SwapMaterial(meshesTransform, "mesh_StripperClipBullets", nextBulletMaterial);
    }
}