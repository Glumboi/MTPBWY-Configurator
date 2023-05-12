using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class Mod
{
    private static readonly string[] _potatoLines = new[]
    {
        "r.Streaming.MinMipForSplitRequest", "r.Streaming.HiddenPrimitiveScale",
        "r.Streaming.AmortizeCPUToGPUCopy", "r.Streaming.MaxNumTexturesToStreamPerFrame",
        "r.Streaming.NumStaticComponentsProcessedPerFrame", "r.Streaming.FramesForFullUpdate"
    };

    private static readonly string[] _potatoVals = new[]
    {
        "0",
        "0.5",
        "1",
        "2",
        "2",
        "1"
    };    
    
    private static readonly string[] _experimentalStutterFixes = new[]
    {
        "s.ForceGCAfterLevelStreamedOut", 
        "s.ContinuouslyIncrementalGCWhileLevelsPendingPurge",
        "r.ShaderPipelineCache.PrecompileBatchTime"
    };

    private static readonly string _experimentalStutterFixesVals = "0";
    
    
    private static void ToggleTAAGen5(bool enabled, ref IniFile ini)
    {
        ini.Write("r.TemporalAA.Algorithm", enabled ? "1" : "0", "SystemSettings");
    }
    
    private static void ToggleTAAUpscaling(bool enabled, ref IniFile ini)
    {
        ini.Write("r.TemporalAA.Upsampling", enabled ? "1" : "0", "SystemSettings");
    }
    
    private static void ToggleTAASettings(bool enabledGen5, bool enabledUpscaling, ref IniFile ini)
    {
        ToggleTAAGen5(enabledGen5, ref ini);
        ToggleTAAUpscaling(enabledUpscaling, ref ini);
    }  

    private static void ChangeTAARes(int screenPercentageValue, ref IniFile ini)
    {
        ini.Write("r.ScreenPercentage", screenPercentageValue.ToString(), "SystemSettings");
    }

    private static void ToggleIniValueFromSliderValue(string key, string section, string value, bool disabled, ref IniFile ini)
    {
        if (disabled)
        {
            ini.DeleteKey(key, section);
            return;
        }

        ini.Write(key, value, section);
    }
    
    
    private static void SetIniVariable(
        string key,
        string section,
        string value,
        bool disabled,
        ref IniFile ini)
    {
        if (disabled)
        {
            ini.DeleteKey(key, section);
            return;
        }

        ini.Write(key, value, section);
    }

    private static void ToggleIniVariable(
        string key,
        string section,
        bool disabled,
        ref IniFile ini)
    {
        if (disabled)
        {
            //r.BloomQuality=0
            ini.Write(key, "0", section);
            return;
        }

        ini.DeleteKey(key, section);
    }

    private static void TogglePotatoTextures(bool enabled, ref IniFile ini)
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

    private static void DisableFog(bool disabled, ref IniFile ini)
    {
        ToggleIniVariable("r.Fog", "SystemSettings", disabled, ref ini);
        ToggleIniVariable("r.VolumetricFog", "SystemSettings", disabled, ref ini);
    }

    private static void EnableLimitPoolSizeToVram(bool disabled, ref IniFile ini)
    {
        SetIniVariable("r.Streaming.LimitPoolSizeToVRAM", "SystemSettings", "1", disabled, ref ini);
    }

    private static void EnableExperimentalStutterFix(bool disabled, ref IniFile ini)
    {
        for (int i = 0; i < _experimentalStutterFixes.Length; i++)
        {
            ToggleIniVariable(_experimentalStutterFixes[i], "SystemSettings", disabled, ref ini);
        }
    }

    public static void Install(
        bool buildOnly,
        bool iniOnly,
        IniFile tempIni,
        PoolSize poolSize,
        string gameDir,
        int taaResolution,
        bool taaUpscaling,
        bool taaGen5,
        bool disableBloom,
        bool disableLensFlare, 
        bool potatoTextures,
        int toneMapperSharpening,
        bool disableDOF,
        bool disableFog,
        int viewDistance,
        bool useExperimentalStutterFix,
        bool disableAntiAliasing,
        bool enablePoolSizeToVramLimit)
    {
        if(string.IsNullOrWhiteSpace(gameDir) && !buildOnly) return;
        
        string pakCreator = Path.Combine(tempIni.EXE, "PakCreator");
        string pakIniLocation = Path.Combine(pakCreator, @"\pakchunk99-Mods_MayThePerformanceBeWithYou_P\SwGame\Config");
        string tempIniPath = tempIni.Path;
        string newIni = Path.Combine(pakCreator + pakIniLocation, "DefaultEngine.ini");

        int trueToneMapperSharpening = toneMapperSharpening / 10;
        float trueViewDistance = viewDistance / 100f;

        ToggleIniVariable("r.BloomQuality", "SystemSettings", disableBloom, ref tempIni);
        ToggleIniVariable("r.LensFlareQuality", "SystemSettings", disableLensFlare, ref tempIni);
        ToggleIniVariable("r.DepthOfFieldQuality", "SystemSettings", disableDOF, ref tempIni);
        ToggleIniVariable("r.PostProcessAAQuality", "SystemSettings", disableAntiAliasing, ref tempIni);

        SetIniVariable("r.Streaming.PoolSize", "SystemSettings", poolSize.PoolSizeMatchingVram.ToString(), false, ref tempIni);

        EnableExperimentalStutterFix(useExperimentalStutterFix, ref tempIni);
        
        DisableFog(disableFog, ref tempIni);
        
        ToggleIniValueFromSliderValue("r.Tonemapper.Sharpen", "SystemSettings",trueToneMapperSharpening.ToString(), 
            trueToneMapperSharpening < 1, ref tempIni);     
        
        ToggleIniValueFromSliderValue("r.ViewDistanceScale", "SystemSettings", trueViewDistance.ToString("0.00").Replace(',', '.'), 
            trueViewDistance == 0, ref tempIni);

        TogglePotatoTextures(potatoTextures, ref tempIni);
        
        ChangeTAARes(taaResolution, ref tempIni);

        ToggleTAASettings(taaGen5, taaUpscaling, ref tempIni);
        
        EnableLimitPoolSizeToVram(!enablePoolSizeToVramLimit, ref tempIni);

        if (!File.Exists(tempIniPath)) return;

        if(iniOnly) return;

        File.Copy(tempIniPath, newIni, true);

        Process.Start(Path.Combine(tempIni.EXE, @"PakCreator\CreateMod.bat")).WaitForExit();
        
        if (buildOnly)
        {
            //"explorer.exe", _saveLocation
            Process.Start("explorer.exe", Path.Combine(tempIni.EXE, @"PakCreator"));
            return;
        }
        
        File.Copy(Path.Combine(tempIni.EXE, @"PakCreator\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"),
            Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"), true);
    }

    public static bool IsModInstalled(string gameDir)
    {
        if (gameDir == null || string.IsNullOrWhiteSpace(gameDir))
        {
            return false;
        }
        
        return File.Exists(Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }

    public static void Uninstall(string gameDir)
    {
        File.Delete(Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }
}