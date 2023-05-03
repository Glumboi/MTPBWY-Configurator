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

    private static void TogglePostProcessingEffect(
        string key,
        string section,
        bool disabled,
        ref IniFile ini)
    {
        if (disabled)
        {
            //r.BloomQuality=0
            ini.Write(key, "0", section);
        }
        else
        {
            ini.DeleteKey(key, section);
        }
    }

    private static void TogglePotatoTextures(bool enabled, ref IniFile ini)
    {
        if (enabled)
        {
            for (var index = 0; index < _potatoLines.Length; index++)
            {
                ini.Write(_potatoLines[index], _potatoVals[index], "SystemSettings");
            }
        }
        else
        {
            for (var index = 0; index < _potatoLines.Length; index++)
            {
                ini.DeleteKey(_potatoLines[index], "SystemSettings");
            }
        }
    }

    public static void Install(
        IniFile tempIni, 
        string gameDir,
        int taaResolution, 
        bool lqTaa, 
        bool disableBloom, 
        bool disableLensFlare, bool potatoTextures)
    {
        string pakCreator = Path.Combine(tempIni.EXE, "PakCreator");
        string pakIniLocation = Path.Combine(pakCreator, @"\pakchunk99-Mods_MayThePerformanceBeWithYou_P\SwGame\Config");
        string tempIniPath = tempIni.Path;
        string newIni = Path.Combine(pakCreator + pakIniLocation, "DefaultEngine.ini");
        
        ChangeTAARes(taaResolution, ref tempIni);
        ToggleLqTAA(lqTaa, ref tempIni);
        
        TogglePostProcessingEffect("r.BloomQuality", "SystemSettings", disableBloom, ref tempIni);
        TogglePostProcessingEffect("r.LensFlareQuality", "SystemSettings", disableLensFlare, ref tempIni);
        TogglePotatoTextures(potatoTextures, ref tempIni);
    
        if(!File.Exists(tempIniPath)) return;
        
        File.Copy(tempIniPath, newIni , true);

        Process.Start(Path.Combine(tempIni.EXE, @"PakCreator\CreateMod.bat")).WaitForExit();

        File.Copy(Path.Combine(tempIni.EXE, @"PakCreator\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"),
            Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"), true);
    }

    public static void Uninstall(string gameDir)
    {
        File.Delete(Path.Combine(gameDir, @"SwGame\Content\Paks\pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak"));
    }
}