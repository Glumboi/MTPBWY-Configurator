using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class Web
{
    public static void CreateLocalTextFileFromRawWebDoc(string url, string destFile)
    {
        using WebClient client = new WebClient();
        string str = client.DownloadString(url);

        try
        {
            File.WriteAllText(destFile, str);
        }
        catch
        {
            return; //ignore
        }
    }
}