using System.Diagnostics;
using System.IO;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class Mod
{
    private static string[] _potatoLines = new[]
    {
        "r.Streaming.MinMipForSplitRequest", "r.Streaming.HiddenPrimitiveScale",
        "r.Streaming.AmortizeCPUToGPUCopy", "r.Streaming.MaxNumTexturesToStreamPerFrame",
        "r.Streaming.NumStaticComponentsProcessedPerFrame", "r.Streaming.FramesForFullUpdate"
    };

    private static string[] _potatoVals = new[]
    {
        "0",
        "0.5",
        "1",
        "2",
        "2",
        "1"
    };

    private static void ToggleLqTAA(bool enabled, ref IniFile ini)
    {
        if (enabled)
        {
            ini.Write("r.TemporalAA.Upsampling", "1", "SystemSettings");
            ini.Write("r.TemporalAA.Algorithm", "1", "SystemSettings");
            return;
        }

        ini.Write("r.TemporalAA.Upsampling", "0", "SystemSettings");
        ini.Write("r.TemporalAA.Algorithm", "0", "SystemSettings");
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

    public static void Install(
        IniFile tempIni,
        string gameDir,
        int taaResolution,
        bool lqTaa,
        bool disableBloom,
        bool disableLensFlare, 
        bool potatoTextures,
        int toneMapperSharpening,
        bool disableDOF,
        bool disableFog,
        int viewDistance)
    {
        string pakCreator = Path.Combine(tempIni.EXE, "PakCreator");
        string pakIniLocation = Path.Combine(pakCreator, @"\pakchunk99-Mods_MayThePerformanceBeWithYou_P\SwGame\Config");
        string tempIniPath = tempIni.Path;
        string newIni = Path.Combine(pakCreator + pakIniLocation, "DefaultEngine.ini");

        int trueToneMapperSharpening = toneMapperSharpening / 10;
        float trueViewDistance = viewDistance / 100f;

        ChangeTAARes(taaResolution, ref tempIni);
        ToggleLqTAA(lqTaa, ref tempIni);

        ToggleIniVariable("r.BloomQuality", "SystemSettings", disableBloom, ref tempIni);
        ToggleIniVariable("r.LensFlareQuality", "SystemSettings", disableLensFlare, ref tempIni);
        ToggleIniVariable("r.DepthOfFieldQuality", "SystemSettings", disableDOF, ref tempIni);
        DisableFog(disableFog, ref tempIni);
        
        ToggleIniValueFromSliderValue("r.Tonemapper.Sharpen", "SystemSettings",trueToneMapperSharpening.ToString(), 
            trueToneMapperSharpening < 1, ref tempIni);     
        
        ToggleIniValueFromSliderValue("r.ViewDistanceScale", "SystemSettings", trueViewDistance.ToString("0.00").Replace(',', '.'), 
            trueViewDistance == 0, ref tempIni);
        
        TogglePotatoTextures(potatoTextures, ref tempIni);

        if (!File.Exists(tempIniPath)) return;

        File.Copy(tempIniPath, newIni, true);

        Process.Start(Path.Combine(tempIni.EXE, @"PakCreator\CreateMod.bat")).WaitForExit();

        File.Copy(Path.Combine(tempIni.EXE, @"PakCreator\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"),
            Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"), true);
    }

    public static bool IsModInstalled(string gameDir)
    {
        return File.Exists(Path.Combine(gameDir,
            @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }

    public static void Uninstall(string gameDir)
    {
        File.Delete(Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }
}