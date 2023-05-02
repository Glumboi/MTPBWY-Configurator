using System;
using System.Collections.Generic;
using PasteBinDataBaseManager;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class PresetDataBase
{
    private DataBase _dataBase;
    public PresetDataBase(string url)
    {
        _dataBase = new DataBase(new Entry[] { }, url);
    }

    public List<Preset> GetPresets()
    {
        List<Preset> result = new List<Preset>();
        
        foreach (var entry in _dataBase.Entries)
        {
            result.Add(new Preset(entry.GetValueOfType("Link"), entry.GetIdentifier()));
        }

        return result;
    }
}