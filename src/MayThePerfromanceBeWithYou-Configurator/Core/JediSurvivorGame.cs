using System.Diagnostics;
using Microsoft.Win32;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

internal class JediSurvivorGame
{
    private string _path = string.Empty;

    public string Path
    {
        get => _path;
        set => _path = value;
    }

    public string GameExecutable => System.IO.Path.Combine(Path, "SwGame\\Binaries\\Win64\\JediSurvivor.exe");

    public JediSurvivorGame(string path = "") => Path = string.IsNullOrWhiteSpace(path) ? GetJediSurvivorPath() : path;

    public void Launch() => Process.Start(GameExecutable);

    public string GetJediSurvivorPath()
    {
        var regFolder = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Respawn\\Jedi Survivor";

        return GetValue<string>(regFolder, "Install Dir");
    }

    private T GetValue<T>(string registryKeyPath, string value, T defaultValue = default(T))
    {
        T retVal = default(T);

        retVal = (T)Registry.GetValue(registryKeyPath, value, defaultValue);

        return retVal;
    }
}