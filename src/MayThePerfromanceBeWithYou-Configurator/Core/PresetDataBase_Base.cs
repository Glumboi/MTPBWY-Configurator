using System.Collections.Generic;
using PasteBinDatabaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public abstract class PresetDatabase_Base
{
    public abstract Database PasteDatabase
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

        foreach (var entry in PasteDatabase.Entries)
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