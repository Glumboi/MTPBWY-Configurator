using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Shapes;
using PasteBinDatabaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class CustomPresetsDatabase : PresetDatabase_Base
{
    public override Database PasteDatabase
    {
        get;
        protected set;
    }

    public override string URL
    {
        get;
        protected set;
    }

    public CustomPresetsDatabase(string path)
    {
        if (!File.Exists(path)) File.Create(path).Close();

        URL = path;
        PasteDatabase = new Database(new Entry[] { }, path);
    }

    public override List<Preset> GetPresets()
    {
        List<Preset> result = new List<Preset>();

        foreach (var entry in PasteDatabase.Entries)
        {
            result.Add(new Preset(entry.GetValueOfType("Path"), entry.GetIdentifier()));
        }

        return result;
    }
}