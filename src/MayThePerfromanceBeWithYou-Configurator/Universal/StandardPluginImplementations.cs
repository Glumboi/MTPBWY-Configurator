using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.CustomSettings;

namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public abstract class StandardPluginImplementations : IPlugin
{
    //A bit of a messy job with the whole plugin system, need to improve it further soon

    public abstract string PluginDebugIdentifier { get; set; }

    public virtual void EntryPoint(object[] args)
    {
        NotImplementedLogMsg("EntryPoint");
    }

    public virtual void Install(bool buildOnly, bool iniOnly, IniFile tempIni, PoolSize poolSize, string gameDir,
        CustomSetting[] modSettings)
    {
        NotImplementedLogMsg("Install");
    }

    public virtual bool IsModInstalled(string gameDir)
    {
        NotImplementedLogMsg("IsModInstalled");
        return false;
    }

    public virtual void Uninstall(string gameDir)
    {
        NotImplementedLogMsg("Uninstall");
    }

    public virtual string GetGamePath()
    {
        return "";
    }

    public virtual void LaunchGame(string gameDir)
    {
        NotImplementedLogMsg("LaunchGame");
    }

    public virtual void OpenGameSaveLocation()
    {
        NotImplementedLogMsg("OpenGameSaveLocation");
    }

    public virtual bool DoesSaveDirectoryExist()
    {
        NotImplementedLogMsg("DoesSaveDirectoryExist");
        return false;
    }

    public virtual void SetupPakCreator(string[] subFolders)
    {
        string localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string pakCreatorPath = $"{localPath}\\PakCreator";
        string gameStructureLocation = $"{pakCreatorPath}\\pakchunk99-Mods_MayThePerformanceBeWithYou_P";

        if (!Directory.Exists(gameStructureLocation)) Directory.CreateDirectory(gameStructureLocation);

        DirectoryInfo di = new DirectoryInfo(gameStructureLocation);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete(); 
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true); 
        }
        
        string? lastCreated = null;
        for (int i = 0; i < subFolders.Length; i++)
        {
            if (lastCreated != null)
            {
                lastCreated = Directory.CreateDirectory($"{lastCreated}\\{subFolders[i]}").FullName;
                continue;
            }

            lastCreated = Directory.CreateDirectory($"{gameStructureLocation}\\{subFolders[i]}").FullName;
        }
    }

    protected void NotImplementedLogMsg(string nameOfFeature)
    {
        AppLogging.SayToLogFile($"Feature {nameOfFeature} is not implemented", AppLogging.LogFileMsgType.WARNING,
            PluginDebugIdentifier);
    }
}