using System;
using System.Diagnostics;
using System.IO;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.CustomSettings;
using MTPBWY_U_JediSurvivor;
using NvAPIWrapper.Native.GPU.Structures;

namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public class DefaultPlugin : StandardPluginImplementations
{
    public override string PluginDebugIdentifier { get; set; }
    private string[] _pakCreatorSubFolders;
    private string _gameDefaultEngineIniLocation;
    private string _gamesPaksLocation;
    private string _gamesExeLocation;
    private string _gameSavePath;
    private string _iniName;
    private const string _modPak = "pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak";

    private readonly string[] _potatoLines = new[]
    {
        "r.Streaming.MinMipForSplitRequest",
        "r.Streaming.HiddenPrimitiveScale",
        "r.Streaming.AmortizeCPUToGPUCopy",
        "r.Streaming.MaxNumTexturesToStreamPerFrame",
        "r.Streaming.NumStaticComponentsProcessedPerFrame",
        "r.Streaming.FramesForFullUpdate"
    };

    private readonly string[] _potatoVals = new[]
    {
        "0",
        "0.5",
        "1",
        "2",
        "2",
        "1"
    };

    private readonly string[] _experimentalStutterFixes = new[]
    {
        "s.ForceGCAfterLevelStreamedOut",
        "s.ContinuouslyIncrementalGCWhileLevelsPendingPurge",
        "r.ShaderPipelineCache.PrecompileBatchTime"
    };

    private readonly string _experimentalStutterFixesVals = "0";

    public DefaultPlugin()
    {
    }

    public DefaultPlugin(
        string[] pakCreatorSubFolders,
        string pluginDebugIdentifier,
        string gameDefaultEngineIniLocation,
        string gamesPaksLocation,
        string gamesExeLocation,
        string gameSavePath,
        string iniName)
    {
        _pakCreatorSubFolders = pakCreatorSubFolders;
        _gameDefaultEngineIniLocation = gameDefaultEngineIniLocation;
        _gamesExeLocation = gamesExeLocation;
        _gamesPaksLocation = gamesPaksLocation;
        _gameSavePath = gameSavePath;
        _iniName = iniName;
        PluginDebugIdentifier = pluginDebugIdentifier;
    }

    public override void EntryPoint(object[] args)
    {
        //Use this if you need something to load up upon Plugin initialization
        AppLogging.SayToLogFile("Initialized!", AppLogging.LogFileMsgType.INFO, PluginDebugIdentifier);
        SetupPakCreator(_pakCreatorSubFolders); //Used for games that require a defaultengine.ini
    }

    public override void Install(bool buildOnly, bool iniOnly, IniFile tempIni, PoolSize poolSize, string gameDir,
        CustomSetting[] modSettings)
    {
        if (string.IsNullOrWhiteSpace(gameDir) && !buildOnly) return;

        string pakCreator = Path.Combine(tempIni.EXE, "PakCreator");
        string pakIniLocation = pakCreator + "\\pakchunk99-Mods_MayThePerformanceBeWithYou_P";

        for (int i = 0; i < _pakCreatorSubFolders.Length; i++)
        {
            pakIniLocation += Path.Combine($"\\{_pakCreatorSubFolders[i]}");
        }

        string tempIniPath = tempIni.Path;
        string newIni = pakIniLocation + $"\\{_iniName}.ini";

        //int trueToneMapperSharpening = modSettings.ToneMapperSharpening / 10;
        //float trueViewDistance = modSettings.ViewDistance / 100f;

        foreach (var setting in modSettings)
        {
            if (setting.JsonData.DefaultValue != setting.JsonData.InitialDefault)
            {
                WriteIniVariable(setting.JsonData.SettingKey, setting.JsonData.SettingSection,
                    setting.JsonData.DefaultValue, tempIni);
            }
        }

        /* ToggleIniVariable("r.BloomQuality", "SystemSettings", modSettings.DisableBloom, tempIni);
         ToggleIniVariable("r.LensFlareQuality", "SystemSettings", modSettings.DisableLensFlare, tempIni);
         ToggleIniVariable("r.DepthOfFieldQuality", "SystemSettings", modSettings.DisableDof, tempIni);
         ToggleIniVariable("r.PostProcessAAQuality", "SystemSettings", modSettings.DisableAntiAliasing, tempIni);

         SetIniVariable("r.Streaming.PoolSize", "SystemSettings", poolSize.PoolSizeMatchingVram.ToString(), false,
             tempIni);
         EnableExperimentalStutterFix(modSettings.UseExperimentalStutterFix, tempIni);
         DisableFog(modSettings.DisableFog, tempIni);

         ToggleIniValueFromSliderValue("r.Tonemapper.Sharpen", "SystemSettings", trueToneMapperSharpening.ToString(),
             trueToneMapperSharpening < 1, tempIni);
         ToggleIniValueFromSliderValue("r.ViewDistanceScale", "SystemSettings",
             trueViewDistance.ToString("0.00").Replace(',', '.'),
             trueViewDistance == 0, tempIni);

         TogglePotatoTextures(modSettings.PotatoTextures, tempIni);
         ChangeTAARes(modSettings.TaaSettings.TaaResolution, tempIni);
         ToggleTAASettings(modSettings.TaaSettings.TaaGen5, modSettings.TaaSettings.TaaUpscaling, tempIni);
         EnableLimitPoolSizeToVram(!modSettings.EnablePoolSizeToVramLimit, tempIni);
         ToggleRtFixes(modSettings.RtFixes, tempIni);

        */
        string newTempIniPath = tempIni.Path + ".new";
        IniMerger.MergeInis(
            File.ReadAllText(tempIniPath),
            File.ReadAllText(_gameDefaultEngineIniLocation),
            newTempIniPath,
            "SystemSettings");

        if (!File.Exists(newTempIniPath)) return;
        if (iniOnly) return;

        File.Copy(newTempIniPath, newIni, true);
        Process.Start(Path.Combine(tempIni.EXE, @"PakCreator\CreateMod.bat")).WaitForExit();
        File.Delete(newTempIniPath);

        if (buildOnly)
        {
            //"explorer.exe", _saveLocation
            Process.Start("explorer.exe", Path.Combine(tempIni.EXE, @"PakCreator"));
            return;
        }

        File.Copy(Path.Combine(tempIni.EXE, $@"PakCreator\{_modPak}"), gameDir + @$"\{_gamesPaksLocation}\{_modPak}",
            true);
    }


    public override void Uninstall(string gameDir)
    {
        File.Delete(Path.Combine(gameDir, $@"{_gamesPaksLocation}\{_modPak}"));
    }

    public override void LaunchGame(string gameDir)
    {
        Process.Start(gameDir + $"\\{_gamesExeLocation}");
    }

    public override bool IsModInstalled(string gameDir)
    {
        if (gameDir == null || string.IsNullOrWhiteSpace(gameDir))
        {
            return false;
        }

        return File.Exists(Path.Combine(gameDir,
            $@"{_gamesPaksLocation}\{_modPak}"));
    }

    public override bool DoesSaveDirectoryExist()
    {
        return Directory.Exists(_gameSavePath.Contains('%')
            ? Environment.ExpandEnvironmentVariables(_gameSavePath)
            : _gameSavePath);
    }

    public override void OpenGameSaveLocation()
    {
        try
        {
            Process.Start("explorer.exe", _gameSavePath.Contains('%')
                ? Environment.ExpandEnvironmentVariables(_gameSavePath)
                : _gameSavePath);
        }
        catch
        {
            AppLogging.SayToLogFile($"Access to save location {_gameSavePath} is forbidden without admin rights",
                AppLogging.LogFileMsgType.WARNING,
                PluginDebugIdentifier);
        }
    }

    private void ToggleTAAGen5(bool enabled, IniFile ini)
    {
        ini.Write("r.TemporalAA.Algorithm", enabled ? "1" : "0", "SystemSettings");
    }

    private void ToggleTAAUpscaling(bool enabled, IniFile ini)
    {
        ini.Write("r.TemporalAA.Upsampling", enabled ? "1" : "0", "SystemSettings");
    }

    private void ToggleTAASettings(bool enabledGen5, bool enabledUpscaling, IniFile ini)
    {
        ToggleTAAGen5(enabledGen5, ini);
        ToggleTAAUpscaling(enabledUpscaling, ini);
    }

    private void ChangeTAARes(int screenPercentageValue, IniFile ini)
    {
        ini.Write("r.ScreenPercentage", screenPercentageValue.ToString(), "SystemSettings");
    }

    private void ToggleIniValueFromSliderValue(string key, string section, string value, bool disabled, IniFile ini)
    {
        if (disabled)
        {
            ini.DeleteKey(key, section);
            return;
        }

        ini.Write(key, value, section);
    }

    private void SetIniVariable(
        string key,
        string section,
        string value,
        bool disabled,
        IniFile ini)
    {
        if (disabled)
        {
            ini.DeleteKey(key, section);
            return;
        }

        ini.Write(key, value, section);
    }

    private void WriteIniVariable(string key, string section, string value, IniFile ini)
    {
        ini.Write(key, value, section);
    }

    private void ToggleIniVariable(
        string key,
        string section,
        bool disabled,
        IniFile ini)
    {
        if (disabled)
        {
            //r.BloomQuality=0
            ini.Write(key, "0", section);
            return;
        }

        ini.DeleteKey(key, section);
    }

    private void TogglePotatoTextures(bool enabled, IniFile ini)
    {
        if (enabled)
        {
            for (var index = 0; index < _potatoLines.Length; index++)
            {
                ini.Write(_potatoLines[index], _potatoVals[index], "SystemSettings");
            }

            return;
        }

        for (var index = 0; index < _potatoLines.Length; index++)
        {
            ini.DeleteKey(_potatoLines[index], "SystemSettings");
        }
    }

    private void DisableFog(bool disabled, IniFile ini)
    {
        ToggleIniVariable("r.Fog", "SystemSettings", disabled, ini);
        ToggleIniVariable("r.VolumetricFog", "SystemSettings", disabled, ini);
    }

    private void ToggleRtFixes(bool enabled, IniFile ini)
    {
        if (enabled)
        {
            ini.Write("r.RayTracing.Geometry.Landscape", "0", "SystemSettings");
            ini.Write("r.HZBOcclusion", "1", "SystemSettings");
            ini.Write("r.AllowOcclusionQueries", "1", "SystemSettings");
            return;
        }

        ini.DeleteKey("r.RayTracing.Geometry.Landscape");
        ini.DeleteKey("r.HZBOcclusion");
        ini.DeleteKey("r.AllowOcclusionQueries");
    }

    private void EnableLimitPoolSizeToVram(bool disabled, IniFile ini)
    {
        SetIniVariable("r.Streaming.LimitPoolSizeToVRAM", "SystemSettings", "1", disabled, ini);
    }

    private void EnableExperimentalStutterFix(bool disabled, IniFile ini)
    {
        for (int i = 0; i < _experimentalStutterFixes.Length; i++)
        {
            ToggleIniVariable(_experimentalStutterFixes[i], "SystemSettings", disabled, ini);
        }
    }
}