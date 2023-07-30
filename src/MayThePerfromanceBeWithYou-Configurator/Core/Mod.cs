using System.Diagnostics;
using System.IO;
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

    private static void ToggleTAAGen5(bool enabled, IniFile ini)
    {
        ini.Write("r.TemporalAA.Algorithm", enabled ? "1" : "0", "SystemSettings");
    }

    private static void ToggleTAAUpscaling(bool enabled, IniFile ini)
    {
        ini.Write("r.TemporalAA.Upsampling", enabled ? "1" : "0", "SystemSettings");
    }

    private static void ToggleTAASettings(bool enabledGen5, bool enabledUpscaling, IniFile ini)
    {
        ToggleTAAGen5(enabledGen5, ini);
        ToggleTAAUpscaling(enabledUpscaling, ini);
    }

    private static void ChangeTAARes(int screenPercentageValue, IniFile ini)
    {
        ini.Write("r.ScreenPercentage", screenPercentageValue.ToString(), "SystemSettings");
    }

    private static void ToggleIniValueFromSliderValue(string key, string section, string value, bool disabled,
        IniFile ini)
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
        IniFile ini)
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

    private static void TogglePotatoTextures(bool enabled, IniFile ini)
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

    private static void DisableFog(bool disabled, IniFile ini)
    {
        ToggleIniVariable("r.Fog", "SystemSettings", disabled, ini);
        ToggleIniVariable("r.VolumetricFog", "SystemSettings", disabled, ini);
    }

    private static void ToggleRtFixes(bool enabled, IniFile ini)
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

    private static void EnableLimitPoolSizeToVram(bool disabled, IniFile ini)
    {
        SetIniVariable("r.Streaming.LimitPoolSizeToVRAM", "SystemSettings", "1", disabled, ini);
    }

    private static void EnableExperimentalStutterFix(bool disabled, IniFile ini)
    {
        for (int i = 0; i < _experimentalStutterFixes.Length; i++)
        {
            ToggleIniVariable(_experimentalStutterFixes[i], "SystemSettings", disabled, ini);
        }
    }

    public static void Install(
        bool buildOnly,
        bool iniOnly,
        IniFile tempIni,
        PoolSize poolSize,
        string gameDir)
        //ModSettings modSettings)
    {
        if (string.IsNullOrWhiteSpace(gameDir) && !buildOnly) return;

        string pakCreator = Path.Combine(tempIni.EXE, "PakCreator");
        string pakIniLocation =
            Path.Combine(pakCreator, @"\pakchunk99-Mods_MayThePerformanceBeWithYou_P\SwGame\Config");
        string tempIniPath = tempIni.Path;
        string newIni = Path.Combine(pakCreator + pakIniLocation, "DefaultEngine.ini");

       /* int trueToneMapperSharpening = modSettings.ToneMapperSharpening / 10;
        float trueViewDistance = modSettings.ViewDistance / 100f;

        ToggleIniVariable("r.BloomQuality", "SystemSettings", modSettings.DisableBloom, tempIni);
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

        if (!File.Exists(tempIniPath)) return;
*/
        if (iniOnly) return;

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

        return File.Exists(Path.Combine(gameDir,
            @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }

    public static void Uninstall(string gameDir)
    {
        File.Delete(Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }
}