using ExtendedWeaponry.Components;

namespace ExtendedWeaponry.Utilities;

internal class AmmoUtilities
{
    internal static Color AmmoTypeColours(AmmoType ammoType)
    {
        return ammoType switch
        {
            AmmoType.ArmorPiercing => Color.green,
            AmmoType.FullMetalJacket => Color.yellow,
            AmmoType.Unspecified => Color.white,
            _ => Color.white,
        };
    }

    internal static string AmmoTypeLocalization(AmmoType ammoType)
    {
        return ammoType switch
        {
            AmmoType.ArmorPiercing => Localization.Get("GAMEPLAY_ArmorPiercing"),
            AmmoType.FullMetalJacket => Localization.Get("GAMEPLAY_FullMetalJacket"),
            _ => ammoType.ToString(),
        };
    }

    internal static int GetAmmoCountInInventory(AmmoType ammoType, GunItem gunItem)
    {
        Inventory inventory = GameManager.GetInventoryComponent();
        if (inventory == null) return 0;

        int count = 0;
        foreach (var gearItemObject in inventory.m_Items)
        {
            GearItem gearItem = gearItemObject;
            if (gearItem != null && IsValidAmmoType(gearItem, gunItem.m_GearItem))
            {
                AmmoAddon ammoExtension = gearItem.gameObject.GetComponent<AmmoAddon>();
                if (ammoExtension != null && ammoExtension.m_AmmoType == ammoType)
                {
                    count += gearItem.m_StackableItem.m_Units;
                }
            }
        }

        return count;
    }

    internal static Material? GetMaterialForAmmoType(AmmoType ammoType)
    {
        string? prefabName = GetPrefabNameForAmmoType(ammoType);
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

    private static string? GetPrefabNameForAmmoType(AmmoType ammoType)
    {
        return ammoType switch
        {
            AmmoType.ArmorPiercing => "GEAR_RifleAmmoSingleAP",
            AmmoType.FullMetalJacket => "GEAR_RifleAmmoSingle",
            _ => null,
        };
    }

    internal static bool IsValidAmmoType(GearItem gearItem, GearItem weapon)
    {
        if (gearItem == null || weapon == null) return false;
        return gearItem.m_AmmoItem && gearItem.m_StackableItem && gearItem.GetRoundedCondition() != 0 && gearItem.m_AmmoItem.m_AmmoForGunType == weapon.m_GunItem.m_GunType;
    }

    internal static void PrioritizeBulletType(GearItem gearItem, Dictionary<AmmoType, int> ammoTypeCounts)
    {
        AmmoAddon ammoExtension = gearItem.GetComponent<AmmoAddon>();
        if (ammoExtension != null)
        {
            AmmoType ammoType = ammoExtension.m_AmmoType;
            ammoTypeCounts.TryGetValue(ammoType, out int currentCount);
            ammoTypeCounts[ammoType] = currentCount + gearItem.m_StackableItem.m_Units;
        }
    }

    internal static void UpdateAmmoMaterials(Transform meshesTransform, AmmoManager ammoManager, Material nextAmmoMaterial)
    {
        int loadedBullets = ammoManager.GetLoadedAmmoCount();
        string ammoMeshName = loadedBullets == 1 ? "mesh_bullet_a" : "mesh_bullet_b";
        TextureSwapper.SwapMaterial(meshesTransform, ammoMeshName, nextAmmoMaterial);
        TextureSwapper.SwapMaterial(meshesTransform, "mesh_StripperClipBullets", nextAmmoMaterial);
    }

    internal static void UpdateAmmoSprites(GunItem gunItem, UISprite[] ammoSprites) // Issue with flare gun ammo sprite copying last color loaded from other weapons.
    {
        if (gunItem.name.Contains("GEAR_FlareGun") && ammoSprites.Length > 0)
        {
            ammoSprites[0].color = Color.white;
        }
        else
        {
            UpdateAmmoSpriteColors(gunItem, ammoSprites);
        }
    }

    private static void UpdateAmmoSpriteColors(GunItem gunItem, UISprite[] ammoSprites)
    {
        AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
        if (ammoManager == null) return;

        for (int i = 0; i < ammoSprites.Length; i++)
        {
            ammoSprites[i].color = i < ammoManager.m_Clip.Count ?
                AmmoTypeColours(ammoManager.m_Clip[i]) :
                Color.white;
        }
    }
}