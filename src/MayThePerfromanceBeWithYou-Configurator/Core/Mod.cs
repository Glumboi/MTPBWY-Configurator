using System.Diagnostics;
using System.IO;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class Mod
{
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
    
    public static void Install(IniFile tempIni, string gameDir, int taaResolution, bool lqTaa)
    {
        string pakCreator = Path.Combine(tempIni.EXE, "PakCreator");
        string pakIniLocation = Path.Combine(pakCreator, @"\pakchunk99-Mods_MayThePerformanceBeWithYou_P\SwGame\Config");
        string tempIniPath = tempIni.Path;
        string newIni = Path.Combine(pakCreator + pakIniLocation, "DefaultEngine.ini");
        
        ChangeTAARes(taaResolution, ref tempIni);
        ToggleLqTAA(lqTaa, ref tempIni);
        
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