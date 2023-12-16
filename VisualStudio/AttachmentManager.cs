using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp(false)]
public class AttachmentManager : MonoBehaviour
{
    private void Awake()
    {
        if (GetComponent<GunItem>() != null)
        {
            InitializeForGunItem();
        }
        else if (GetComponent<FirstPersonWeapon>() != null)
        {
            InitializeForFirstPersonWeapon();
        }
        Logging.Log("Scopes Attached.");
    }

    private void InitializeForGunItem()
    {
        GearItem scopePrefab = GearItem.LoadGearItemPrefab("GEAR_Scope");
        if (scopePrefab == null) return;

        GameObject scopePoint = new("ScopeAttachmentPoint");
        scopePoint.transform.SetParent(transform);

        scopePoint.transform.localPosition = new Vector3(0, 0.3f, 0.1f);
        scopePoint.transform.localRotation = Quaternion.Euler(0, 180, 0);

        Instantiate(scopePrefab.gameObject, scopePoint.transform.position, scopePoint.transform.rotation, scopePoint.transform);
    }

    private void InitializeForFirstPersonWeapon()
    {
        FirstPersonWeapon fpw = GetComponent<FirstPersonWeapon>();
        if (fpw == null || fpw.m_Renderable == null)
        {
            Logging.LogError("FirstPersonWeapon or m_Renderable is null.");
            return;
        }

        Transform meshesTransform = fpw.m_Renderable.transform.Find("Meshes");
        if (meshesTransform == null) return;

        Transform scopeAttachmentPoint = meshesTransform.Find("ScopeAttachmentPoint");
        if (scopeAttachmentPoint == null)
        {
            GameObject newAttachmentPoint = new("ScopeAttachmentPoint");
            newAttachmentPoint.transform.SetParent(meshesTransform);

            newAttachmentPoint.transform.localPosition = new Vector3(0, 0.11f, 0.2f);       // 0.15f for proper placement
            newAttachmentPoint.transform.localRotation = Quaternion.identity;

            scopeAttachmentPoint = newAttachmentPoint.transform;
        }

        GearItem scopePrefab = GearItem.LoadGearItemPrefab("GEAR_Scope");
        if (scopePrefab != null)
        {
            Instantiate(scopePrefab.gameObject, scopeAttachmentPoint.position, scopeAttachmentPoint.rotation, scopeAttachmentPoint);
        }
        else
        {
            Logging.LogError("Scope prefab not found.");
        }
    }
}