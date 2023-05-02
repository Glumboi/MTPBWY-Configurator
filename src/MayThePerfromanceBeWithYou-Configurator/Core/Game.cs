using Microsoft.Win32;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

internal class Game
{
    public static string GetJediSurvivorPath()
    {
        var regFolder = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Respawn\\Jedi Survivor";

        return GetValue<string>(regFolder, "Install Dir");
    }

    private static T GetValue<T>(string registryKeyPath, string value, T defaultValue = default(T))
    {
        T retVal = default(T);

        retVal = (T)Registry.GetValue(registryKeyPath, value, defaultValue);

        return retVal;
    }
}