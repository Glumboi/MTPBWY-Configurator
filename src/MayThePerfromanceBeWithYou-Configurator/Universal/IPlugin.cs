using System.IO;
using System.Reflection;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.CustomSettings;

namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public interface IPlugin
{
    public void Install(
        bool buildOnly,
        bool iniOnly,
        IniFile tempIni,
        PoolSize poolSize,
        string gameDir,
        CustomSetting[] modSettings);

    public bool IsModInstalled(string gameDir);

    public void Uninstall(string gameDir);

    public string GetGamePath();

    public void LaunchGame(string gameDir);

    public void OpenGameSaveLocation();

    public bool DoesSaveDirectoryExist();
}