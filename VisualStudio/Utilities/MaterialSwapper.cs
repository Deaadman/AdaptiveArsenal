namespace AdaptiveArsenal.Utilities;

internal class MaterialSwapper
{
    internal static Material? GetLineRendererMaterialFromGearItemPrefab(string gearItemName, string childName)
    {
        GearItem gearItemPrefab = GearItem.LoadGearItemPrefab(gearItemName);
        if (gearItemPrefab == null) return null;

        Transform childTransform = gearItemPrefab.transform.Find(childName);
        if (childTransform != null)
        {
            LineRenderer lineRenderer = childTransform.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                return lineRenderer.material;
            }
        }
        return null;
    }
}