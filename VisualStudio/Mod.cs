using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.OnBulletLoaded))]
    private static class TestingClassToSwapAnimationBulletMaterials
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            if (__instance.m_Weapon == null || __instance.m_Weapon.m_GearItem == null) return;

            AmmoManager ammoManager = __instance.m_Weapon.m_GearItem.GetComponent<AmmoManager>();
            if (ammoManager == null) return;

            Material? newMaterial = AmmoManager.GetMaterialForBulletType(ammoManager.GetNextBulletType());
            if (newMaterial == null) return;

            FirstPersonWeapon firstPersonWeapon = __instance.m_Weapon.m_FirstPersonWeaponShoulder.GetComponent<FirstPersonWeapon>();
            if (firstPersonWeapon != null && firstPersonWeapon.m_Renderable != null)
            {
                Transform meshesTransform = firstPersonWeapon.m_Renderable.transform.Find("Meshes");
                if (meshesTransform != null)
                {
                    TextureSwapper.SwapMaterial(meshesTransform, "mesh_bullet_a", newMaterial);
                    TextureSwapper.SwapMaterial(meshesTransform, "mesh_bullet_b", newMaterial);
                    TextureSwapper.SwapMaterial(meshesTransform, "mesh_StripperClipBullets", newMaterial);
                }
            }
        }
    }
}