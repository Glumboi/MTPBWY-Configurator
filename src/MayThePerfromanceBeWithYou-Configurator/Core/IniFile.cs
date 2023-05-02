using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
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
    static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    public IniFile(string IniPath = null)
    {
        if (IniPath.Contains("http"))
        {
            using (WebClient client = new WebClient())
            {
                Stream stream = client.OpenRead(IniPath);
                StreamReader streamReader = new StreamReader(stream);
                Collection<string> stringCollection = new Collection<string>();
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    stringCollection.Add(line);
                }
                File.WriteAllLines("tempIni.ini", stringCollection);
                Path = System.IO.Path.Combine(EXE, "tempIni.ini");
                return;
            }
        }
        Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
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