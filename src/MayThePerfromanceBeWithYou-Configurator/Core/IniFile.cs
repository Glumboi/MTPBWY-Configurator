using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class IniFile   // revision 11
{
    public string Path
    {
        get;
        private set;
    }

    public string EXE => System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    public IniFile(string IniPath = null, bool keepOriginalFile = false)
    {
        string tempIniPath = System.IO.Path.Combine(EXE, "tempIni.ini");
        if (IniPath.Contains("http"))
        {
            Web.CreateLocalTextFileFromRawWebDoc(IniPath, "tempIni.ini");
            Path = tempIniPath;
            return;
        }

        if (!keepOriginalFile)
        {
            File.Copy(IniPath, System.IO.Path.Combine(EXE, "tempIni.ini"), true);
        }
        Path = tempIniPath;
    }

    public string Read(string Key, string Section = null)
    {
        var RetVal = new StringBuilder(255);
        GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
        return RetVal.ToString();
    }

    public void Write(string Key, string Value, string Section = null)
    {
        WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
    }

    public void DeleteKey(string Key, string Section = null)
    {
        Write(Key, null, Section ?? EXE);
    }

    public void DeleteSection(string Section = null)
    {
        Write(null, null, Section ?? EXE);
    }

    public bool KeyExists(string Key, string Section = null)
    {
        return Read(Key, Section).Length > 0;
    }
}