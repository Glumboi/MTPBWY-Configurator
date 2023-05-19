using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class Web
{
    public static void CreateLocalTextFileFromRawWebDoc(string url, string destFile)
    {
        using WebClient client = new WebClient();
        Stream stream = client.OpenRead(url);
        StreamReader streamReader = new StreamReader(stream);
        Collection<string> stringCollection = new Collection<string>();
        string line;

        while ((line = streamReader.ReadLine()) != null)
        {
            stringCollection.Add(line);
        }

        try
        {
            File.WriteAllLines(destFile, stringCollection);
        }
        catch
        {
            // ignored
        }
    }
}