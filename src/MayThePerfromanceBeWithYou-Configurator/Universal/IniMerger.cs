using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPBWY_U_JediSurvivor;

public static class IniMerger
{
    public static void MergeInis(string tempIni, string targetIniContent, string targetFilePath, string sectionName)
    {
        List<string> tempIniLines = tempIni.Split('\n').ToList();
        tempIniLines.Remove($"[{sectionName}]");
        List<string> targetLines = targetIniContent.Split(Environment.NewLine).ToList();
        if (!tempIniLines.Contains($"[{sectionName}]"))
        {
            tempIniLines.Insert(0, $"[{sectionName}]");
        }

        targetLines.InsertRange(targetLines.IndexOf($"[{sectionName}]") + 1, tempIniLines);
        File.WriteAllLines(targetFilePath, targetLines);
    }
}