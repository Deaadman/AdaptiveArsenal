using ExtendedWeaponry.Utilities;

namespace ExtendedWeaponry.Components;

[RegisterTypeInIl2Cpp(false)]
public class PanelHUDAddon : MonoBehaviour
{
    private Panel_HUD? m_Panel_HUD;
    internal GameObject? m_AmmoTypePrefab;
    internal UILabel? m_LabelInspectAmmoType;

    private void Awake()
    {
        m_Panel_HUD = GetComponent<Panel_HUD>();
        AssetBundle? extendedWeaponryAssetBundle = AssetBundleLoader.LoadBundle();

        GameObject? prefab = extendedWeaponryAssetBundle?.LoadAsset<GameObject>("Assets/ExtendedWeaponry/AmmoType.prefab");
        GameObject? testingInstance = Instantiate(prefab, m_Panel_HUD.m_InspectModeDetailsGrid.gameObject.transform);

        if (testingInstance != null)
        {
            m_AmmoTypePrefab = testingInstance;
            m_AmmoTypePrefab.transform.SetSiblingIndex(0);
            m_AmmoTypePrefab.SetActive(false);
            m_LabelInspectAmmoType = m_AmmoTypePrefab.transform.Find("Label_InspectMode_AmmoType")?.GetComponent<UILabel>();
        }

        SetupInspectFade();
    }

    private void SetupInspectFade()
    {
        if (m_Panel_HUD != null && m_LabelInspectAmmoType != null && m_Panel_HUD.m_InspectFadeSequence.Length > 1)
        {
            Panel_HUD.InspectFade inspectFade = m_Panel_HUD.m_InspectFadeSequence[1];

            int newSize = inspectFade.m_FadeElements.Length + 1;
            UIWidget[] newFadeElements = new UIWidget[newSize];

            Array.Copy(inspectFade.m_FadeElements, newFadeElements, inspectFade.m_FadeElements.Length);

            newFadeElements[inspectFade.m_FadeElements.Length] = m_LabelInspectAmmoType;
            inspectFade.m_FadeElements = newFadeElements;

            m_Panel_HUD.m_InspectFadeSequence[1] = inspectFade;
        }
    }
}