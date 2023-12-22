using ExtendedWeaponry.Components;
using System.Text.Json;

namespace ExtendedWeaponry.Utilities;

internal class DataManager
{
    readonly ModDataManager modDataManager = new("Extended Weaponry", true);

    public void Save(AmmoManagerSaveDataProxy data)
    {
        string? dataString;
        dataString = JsonSerializer.Serialize(data);
        modDataManager.Save(dataString);
    }

    public AmmoManagerSaveDataProxy? Load()
    {
        string? dataString = modDataManager.Load();
        if (dataString is null) return null;

        AmmoManagerSaveDataProxy? data = JsonSerializer.Deserialize<AmmoManagerSaveDataProxy>(dataString);
        return data;
    }
}