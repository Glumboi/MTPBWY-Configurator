using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Navigation;
using PasteBinDataBaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class PresetDataBase : PresetDataBase_Base
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
    
    public PresetDataBase(string url)
    {
        URL = url;
        PasteDataBase = new DataBase(new Entry[] { }, url);
    }
}