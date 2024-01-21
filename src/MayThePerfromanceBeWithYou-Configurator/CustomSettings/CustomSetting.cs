using System;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using MayThePerfromanceBeWithYou_Configurator.CustomControls;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;

namespace MayThePerfromanceBeWithYou_Configurator.CustomSettings;

public class CustomSetting
{
    public CustomSettingJson JsonData { get; private set; }
    
    public CustomSetting(string jsonPath)
    {
        JsonData = JsonSerializer.Deserialize<CustomSettingJson>(File.ReadAllText(jsonPath));
    }
}