using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

internal sealed class Mod : MelonMod
{
    [HarmonyPatch(typeof(vp_FPSShooter), nameof(vp_FPSShooter.OnBulletLoaded))]
    private static class TestingClassToSwapAnimationBulletMaterials
    {
        private static void Postfix(vp_FPSShooter __instance)
        {
            if (__instance.m_Weapon == null || __instance.m_Weapon.m_GearItem == null)
            {
                Logging.LogError("m_Weapon or m_GearItem is null in vp_FPSShooter.");
                return;
            }

            AmmoManager ammoManager = __instance.m_Weapon.m_GearItem.GetComponent<AmmoManager>();
            if (ammoManager == null)
            {
                Logging.LogError("AmmoManager component not found on m_GearItem.");
                return;
            }

            Material? newMaterial = AmmoManager.GetMaterialForBulletType(ammoManager.GetNextBulletType());
            if (newMaterial == null)
            {
                Logging.LogError("Failed to get new material for bullet.");
                return;
            }

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
                else
                {
                    Logging.LogError("Meshes GameObject not found in m_Renderable.");
                }
            }
            else
            {
                Logging.LogError("FirstPersonWeapon or m_Renderable is null in vp_FPSShooter.");
            }
        }
    }
}