using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Shapes;
using PasteBinDataBaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class CustomPresetsDataBase : PresetDataBase_Base
{
    public override DataBase PasteDataBase
    {
        get;
        protected set;
    }

    public override string URL
    {
        get;
        protected set;
    }

    public CustomPresetsDataBase(string path)
    {
        if (!File.Exists(path)) File.Create(path).Close(); 
        
        URL = path;
        PasteDataBase = new DataBase(new Entry[] { }, path);
    }

    public override List<Preset> GetPresets()
    {
        List<Preset> result = new List<Preset>();

        foreach (var entry in PasteDataBase.Entries)
        {
            result.Add(new Preset(entry.GetValueOfType("Path"), entry.GetIdentifier()));
        }

        return result;
    }
}