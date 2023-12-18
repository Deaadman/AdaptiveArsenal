using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class EquipItemPopupExtension : MonoBehaviour
{
    private static readonly AssetBundle? ExtendedWeaponryAssetBundle = AssetBundleLoader.LoadBundle("ExtendedWeaponry.Resources.ExtendedWeaponryAssetBundle");

    private EquipItemPopup? m_EquipItemPopup;
    internal GameObject? m_AmmoWidgetExtensionPrefab;
    internal UILabel? m_LabelAmmoCount;
    internal UILabel? m_LabelAmmoType;

    private void Awake()
    {
        m_EquipItemPopup = GetComponent<EquipItemPopup>();

        GameObject? prefab = ExtendedWeaponryAssetBundle?.LoadAsset<GameObject>("Assets/ExtendedWeaponry/AmmoWidgetExtension.prefab");
        GameObject? widgetInstance = Instantiate(prefab, m_EquipItemPopup.m_LabelAmmoReserve.transform.parent);

        if (widgetInstance != null)
        {
            widgetInstance.SetActive(false);
            m_AmmoWidgetExtensionPrefab = widgetInstance;
            m_LabelAmmoCount = widgetInstance.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>();
            m_LabelAmmoType = widgetInstance.transform.Find("Label_AmmoType")?.GetComponent<UILabel>();
        }
    }
}