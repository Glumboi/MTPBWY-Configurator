using System;
using System.Collections.Generic;
using System.Windows.Input;
using MayThePerfromanceBeWithYou_Configurator.CustomSettings;
using Wpf.Ui.Common;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public class ModSettingsViewerViewModel : ViewModelBase
{
    private List<string> _modSettings = new List<string>();

    public List<string> ModSettings
    {
        get => _modSettings;
        set => SetProperty(ref _modSettings, value);
    }

    private Func<List<CustomSetting>> _reloadModSettingsFunction;

    public Func<List<CustomSetting>> ReloadModSettingsFunction
    {
        get => _reloadModSettingsFunction;
        set => SetProperty(ref _reloadModSettingsFunction, value);
    }

    public ICommand ReloadModSettingsCommand { get; internal set; }

    private void CreateReloadModSettingsCommand()
    {
        ReloadModSettingsCommand = new RelayCommand(Reload, () => { return ReloadModSettingsFunction != null; });
    }

    private void Reload()
    {
        ModSettings.Clear();
        var settings = ReloadModSettingsFunction.Invoke();
        foreach (var setting in settings)
        {
            ModSettings.Add(setting.JsonData.SettingName + " | " + setting.JsonData.SettingKey + " | " +
                            setting.JsonData.SettingType + " | " + setting.JsonData.DefaultValue);
        }
    }

    public void InitializeViewModel(List<CustomSetting> settings,
        Func<List<CustomSetting>> reloadModSettingsFunction)
    {
        ReloadModSettingsFunction = reloadModSettingsFunction;
        Reload();
    }

    public ModSettingsViewerViewModel()
    {
        CreateReloadModSettingsCommand();
    }
}