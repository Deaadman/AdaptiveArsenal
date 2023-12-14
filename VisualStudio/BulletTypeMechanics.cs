using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal class BulletTypeMechanics
{
    [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
    public static class ApplyDamageMultiplierPatch
    {
        private static void Prefix(GunItem __instance)
        {
            AmmoManager ammoManager = __instance.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            if (ammoManager.GetCurrentBulletType() == BulletType.ArmorPiercing)
            {

            }
        }
    }
}