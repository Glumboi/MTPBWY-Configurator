using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Navigation;
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
        PasteDatabase = new Database(new Entry[] { }, url);
    }
}