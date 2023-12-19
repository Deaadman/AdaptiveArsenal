using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class EquipItemPopupAddon : MonoBehaviour
{
    private EquipItemPopup? m_EquipItemPopup;
    internal GameObject? m_AmmoWidgetExtensionPrefab;
    internal UILabel? m_LabelAmmoCount;
    internal UILabel? m_LabelAmmoType;

    private void Awake()
    {
        m_EquipItemPopup = GetComponent<EquipItemPopup>();
        AssetBundle? extendedWeaponryAssetBundle = AssetBundleLoader.LoadBundle();

        GameObject? prefab = extendedWeaponryAssetBundle?.LoadAsset<GameObject>("Assets/ExtendedWeaponry/AmmoWidgetExtension.prefab");
        GameObject? widgetInstance = Instantiate(prefab, m_EquipItemPopup.m_LabelAmmoReserve.transform.parent);

        if (widgetInstance != null)
        {
            m_AmmoWidgetExtensionPrefab = widgetInstance;
            m_AmmoWidgetExtensionPrefab.SetActive(false);
            m_LabelAmmoCount = widgetInstance.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>();
            m_LabelAmmoType = widgetInstance.transform.Find("Label_AmmoType")?.GetComponent<UILabel>();
        }
    }
}