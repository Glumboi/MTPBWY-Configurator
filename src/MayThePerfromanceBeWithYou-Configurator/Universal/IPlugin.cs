using MayThePerfromanceBeWithYou_Configurator.Core;

namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public interface IPlugin
{
    public void Install(
        bool buildOnly,
        bool iniOnly,
        IniFile tempIni,
        PoolSize poolSize,
        string gameDir,
        ModSettings modSettings);

    public bool IsModInstalled(string gameDir);

    public void Uninstall(string gameDir);
}