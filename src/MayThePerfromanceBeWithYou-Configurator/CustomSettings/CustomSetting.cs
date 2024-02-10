using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Controls;
using MayThePerfromanceBeWithYou_Configurator.CustomControls;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;

namespace MayThePerfromanceBeWithYou_Configurator.CustomSettings;

public class CustomSetting : INotifyPropertyChanged
{
    public CustomSettingJson JsonData { get; private set; }

    private string _defaultValue;

    public string DefaultValue
    {
        get => _defaultValue;
        set
        {
            if (_defaultValue != value)
            {
                _defaultValue = JsonData.DefaultValue = value;
                OnPropertyChanged(nameof(DefaultValue));
            }
        }
    }

    public CustomSetting(string jsonPath)
    {
        JsonData = JsonSerializer.Deserialize<CustomSettingJson>(File.ReadAllText(jsonPath));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}