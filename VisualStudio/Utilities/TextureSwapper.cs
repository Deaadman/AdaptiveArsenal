namespace ExtendedWeaponry.Utilities;

internal class TextureSwapper
{
    internal static Material[]? GetMaterialsFromGearItemPrefab(string gearItemName)
    {
        GearItem gearItemPrefab = GearItem.LoadGearItemPrefab(gearItemName);
        if (gearItemPrefab == null) return null;

        return GetAllMaterialsInGameObject(gearItemPrefab);
    }

    internal static Material[] GetAllMaterialsInGameObject(GearItem gi)
    {
        Renderer[] renderers = gi.GetComponentsInChildren<Renderer>();
        Material[] materials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }

        return materials;
    }

    internal static void SwapMaterial(Transform parent, string childName, Material newMaterial)
    {
        Transform childTransform = parent.Find(childName);
        if (childTransform != null)
        {
            Renderer renderer = childTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }
    }
}