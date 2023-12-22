using ExtendedWeaponry.Components;

namespace ExtendedWeaponry.Utilities;

internal class AmmoUtilities
{
    internal static Color AmmoTypeColours(AmmoType ammoType) => ammoType switch
    {
        AmmoType.FullMetalJacket => Color.yellow,
        AmmoType.ArmorPiercing => Color.green,
        _ => Color.white,
    };

    internal static string AmmoTypeLocalization(AmmoType ammoType) => ammoType switch
    {
        AmmoType.FullMetalJacket => Localization.Get("GAMEPLAY_FullMetalJacket"),
        AmmoType.ArmorPiercing => Localization.Get("GAMEPLAY_ArmorPiercing"),
        _ => ammoType.ToString(),
    };

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
            AmmoType.FullMetalJacket => "GEAR_RifleAmmoSingle",
            AmmoType.ArmorPiercing => "GEAR_RifleAmmoSingleAP",
            _ => null,
        };
    }

    internal static void UpdateAmmoMaterials(Transform meshesTransform, AmmoManager ammoManager, Material nextAmmoMaterial)
    {
        int loadedBullets = ammoManager.m_Clip.Count;
        string ammoMeshName = loadedBullets == 1 ? "mesh_bullet_a" : "mesh_bullet_b";
        TextureSwapper.SwapMaterial(meshesTransform, ammoMeshName, nextAmmoMaterial);
        TextureSwapper.SwapMaterial(meshesTransform, "mesh_StripperClipBullets", nextAmmoMaterial);
    }

    internal static void UpdateAmmoSprites(GunItem gunItem, UISprite[] ammoSprites)
    {
        AmmoManager ammoManager = gunItem.GetComponent<AmmoManager>();
        if (ammoManager == null) return;
        if (gunItem.name.Contains("GEAR_FlareGun")) return; // Only issue is that the sprite for the FlareGun stays the last colours that was chosen.

        for (int i = 0; i < ammoSprites.Length; i++)
        {
            ammoSprites[i].color = i < ammoManager.m_Clip.Count ? AmmoTypeColours(ammoManager.m_Clip[i]) : Color.white;
        }
    }
}