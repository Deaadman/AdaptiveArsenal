using System.Text;

namespace ExtendedWeaponry.Utilities;

public class Logging
{
    public static void LogStarter()                                                     => Melon<Mod>.Logger.Msg($"Mod loaded with v{Properties.BuildInfo.Version}");
    public static void LogSeperator(params object[] parameters)                         => Melon<Mod>.Logger.Msg("==============================================================================", parameters);
    public static void LogIntraSeparator(string message, params object[] parameters)    => Melon<Mod>.Logger.Msg($"=========================   {message}   =========================", parameters);

    public static void Log(string message, params object[] parameters)                  => Melon<Mod>.Logger.Msg(message, parameters);
    public static void LogDebug(string message, params object[] parameters)             => Melon<Mod>.Logger.Msg($"[DEBUG] {message}", parameters);
    public static void LogWarning(string message, params object[] parameters)           => Melon<Mod>.Logger.Warning(message, parameters);
    public static void LogError(string message, params object[] parameters)             => Melon<Mod>.Logger.Error(message, parameters);
    public static void LogException(string message, Exception exception, params object[] parameters)
    {
        StringBuilder sb = new();

        sb.Append("[EXCEPTION]");
        sb.Append(message);
        sb.AppendLine(exception.Message);

        Melon<Mod>.Logger.Error(sb.ToString(), Color.red, parameters);
    }
}