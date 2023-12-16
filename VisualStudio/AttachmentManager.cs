using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry;

[RegisterTypeInIl2Cpp(false)]
public class AttachmentManager : MonoBehaviour
{
    private GameObject scopeInstance;
    private Transform referenceTransform; // Transform to align the scope with

    private void Awake()
    {
        GearItem scopePrefab = GearItem.LoadGearItemPrefab("GEAR_Scope");
        if (scopePrefab == null)
        {
            Logging.LogError("Scope prefab could not be loaded.");
            return;
        }

        scopeInstance = Instantiate(scopePrefab.gameObject);
        Logging.Log("Scope instance created.");
    }

    private void Start()
    {
        vp_FPSShooter vpfps = GetComponent<vp_FPSShooter>();
        if (vpfps == null)
        {
            Logging.LogWarning("vp_FPSWeapon component not found on the GameObject.");
            return;
        }

        FirstPersonWeapon firstPersonWeapon = vpfps.m_Weapon.m_FirstPersonWeaponShoulder.GetComponent<FirstPersonWeapon>();
        if (firstPersonWeapon == null)
        {
            Logging.LogWarning("FirstPersonWeapon component not found on the GameObject.");
            return;
        }

        if (firstPersonWeapon.m_Renderable == null)
        {
            Logging.LogError("FirstPersonWeapon's m_Renderable is null.");
            return;
        }

        referenceTransform = firstPersonWeapon.m_Renderable.transform.Find("Meshes");
        if (referenceTransform == null)
        {
            Logging.LogError("Reference transform 'Meshes' not found on FirstPersonWeapon's m_Renderable.");
            return;
        }

        Logging.Log("Reference transform for scope alignment set.");
    }

    private void Update()
    {
        if (scopeInstance == null)
        {
            Logging.LogWarning("Scope instance is null in Update.");
            return;
        }

        if (referenceTransform == null)
        {
            Logging.LogWarning("Reference transform is null in Update.");
            return;
        }

        UpdateScopePosition();
    }

    private void UpdateScopePosition()
    {
        Vector3 worldPosition = referenceTransform.position;
        Quaternion worldRotation = referenceTransform.rotation;

        Logging.Log($"Reference Position: {worldPosition}, Reference Rotation: {worldRotation}");

        scopeInstance.transform.position = worldPosition;
        scopeInstance.transform.rotation = worldRotation;

        Logging.Log("Scope position and rotation updated.");

        Debug.DrawLine(worldPosition, worldPosition + worldRotation * Vector3.forward * 2.0f, Color.red);
        Debug.DrawLine(worldPosition, worldPosition + worldRotation * Vector3.up * 2.0f, Color.green);
    }
}