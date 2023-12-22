using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class EquipItemPopupAddon : MonoBehaviour
{
    internal UILabel? m_LabelAmmoCount;
    internal UILabel? m_LabelAmmoType;

    private EquipItemPopup? m_EquipItemPopup;

    private void Awake()
    {
        m_EquipItemPopup = GetComponent<EquipItemPopup>();
        AssetBundle? extendedWeaponryAssetBundle = AssetBundleLoader.LoadBundle();

        GameObject? prefab = extendedWeaponryAssetBundle?.LoadAsset<GameObject>("Assets/ExtendedWeaponry/AmmoWidgetExtension.prefab");
        GameObject? widgetInstance = Instantiate(prefab, m_EquipItemPopup.m_LabelAmmoReserve.transform.parent);

        if (widgetInstance != null)
        {
            m_LabelAmmoCount = widgetInstance.transform.Find("Label_AmmoCount")?.GetComponent<UILabel>();
            m_LabelAmmoType = widgetInstance.transform.Find("Label_AmmoType")?.GetComponent<UILabel>();
        }
    }
}