//using ExtendedWeaponry.Utilities;
//using UnityEngine.AddressableAssets;

//namespace ExtendedWeaponry;

//[RegisterTypeInIl2Cpp(false)]
//public class AttachmentManager : MonoBehaviour
//{
//    private void Awake()
//    {
//        if (GetComponent<GunItem>() != null)
//        {
//            InitializeForGunItem();
//        }
//        else if (GetComponent<vp_FPSWeapon>() != null)
//        {
//            InitializeForFirstPersonWeapon();
//        }
//    }

//    private static void InitializeForGunItem()
//    {
//        GameObject scopePrefab = Addressables.LoadAssetAsync<GameObject>("GEAR_Scope").WaitForCompletion();
//        if (scopePrefab == null) return;

//        Instantiate(scopePrefab.gameObject);
//    }

//    private void InitializeForFirstPersonWeapon()
//    {
//        vp_FPSWeapon vpfps = GetComponent<vp_FPSWeapon>();
//        if (vpfps == null) return;

//        FirstPersonWeapon fpw = vpfps.m_FirstPersonWeaponShoulder;
//        if (fpw == null || fpw.m_Renderable == null) return;

//        Transform meshesTransform = fpw.m_Renderable.transform.Find("Meshes");
//        if (meshesTransform == null) return;

//        GameObject scopePrefab = Addressables.LoadAssetAsync<GameObject>("GEAR_Scope").WaitForCompletion();
//        if (scopePrefab != null)
//        {
//            //PlayerAnimation.SetLayerOnObjectRecursively(scopePrefab, 23);
//            GameObject instantiatedScope = Instantiate(scopePrefab.gameObject, meshesTransform);
//            instantiatedScope.transform.localPosition = new Vector3(0, 0.135f, 0.1f);

//            Logging.Log("Scope prefab instantiated directly under Meshes with adjusted local position.");
//        }
//    }
//}