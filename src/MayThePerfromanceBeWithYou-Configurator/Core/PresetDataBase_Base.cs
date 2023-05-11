using System.Collections.Generic;
using PasteBinDataBaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public abstract class PresetDataBase_Base
{
    public abstract DataBase PasteDataBase
    {
        get;
        protected set;
    }

    public abstract string URL
    {
        get;
        protected set;
    }

    public virtual List<Preset> GetPresets()
    {
        List<Preset> result = new List<Preset>();

        foreach (var entry in PasteDataBase.Entries)
        {
            result.Add(new Preset(entry.GetValueOfType("Link"), entry.GetIdentifier()));
        }

        return result;
    }
    
    public virtual void CreateLocalDatabase()
    {
        Web.CreateLocalTextFileFromRawWebDoc(URL, "LocalDatabase.txt");
    }
}