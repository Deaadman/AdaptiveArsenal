namespace ExtendedWeaponry.Utilities;

internal static class AmmoManager
{
    private static readonly Dictionary<int, BulletType> ammoTypes = [];

    internal static void SetAmmoType(int clipIndex, BulletType bulletType)
    {
        ammoTypes[clipIndex] = bulletType;
        Logging.Log($"Set Ammo Type: Index = {clipIndex}, BulletType = {bulletType}");
    }

    internal static BulletType GetAmmoType(int clipIndex)
    {
        if (ammoTypes.TryGetValue(clipIndex, out BulletType bulletType))
        {
            Logging.Log($"Get Ammo Type: Index = {clipIndex}, Found BulletType = {bulletType}");
            return bulletType;
        }
        else
        {
            Logging.LogWarning($"Get Ammo Type: Index = {clipIndex}, BulletType not found, defaulting to Unspecified");
            return BulletType.Unspecified;
        }
    }
}