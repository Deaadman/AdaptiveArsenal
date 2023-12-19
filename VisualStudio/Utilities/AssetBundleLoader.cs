namespace ExtendedWeaponry.Utilities;

internal static class AssetBundleLoader
{
    private static AssetBundle? m_ExtendedWeaponryAssetBundle;
    private static readonly string m_BundlePath = "ExtendedWeaponry.Resources.ExtendedWeaponryAssetBundle";
    private static bool m_IsBundleLoaded = false;

    internal static AssetBundle? LoadBundle()
    {
        if (!m_IsBundleLoaded)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(m_BundlePath);

            if (stream == null)
            {
                Logging.LogError("Stream is null. Unable to load asset bundle.");
                return null;
            }

            using (MemoryStream memory = new((int)stream.Length))
            {
                stream.CopyTo(memory);
                m_ExtendedWeaponryAssetBundle = AssetBundle.LoadFromMemory(memory.ToArray());
            }

            stream.Dispose();
            m_IsBundleLoaded = true;
        }

        return m_ExtendedWeaponryAssetBundle;
    }
}