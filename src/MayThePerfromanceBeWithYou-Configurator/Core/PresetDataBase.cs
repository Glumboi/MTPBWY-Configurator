using System.Collections.Generic;
using PasteBinDatabaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class PresetDatabase : PresetDatabase_Base
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

    public PresetDatabase(string url)
    {
        URL = url;
        PasteDatabase = new Database(new List<Entry>(), url);
    }
}