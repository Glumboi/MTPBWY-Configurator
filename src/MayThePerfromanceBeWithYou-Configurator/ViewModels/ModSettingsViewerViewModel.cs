using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Documents;
using System.Windows.Input;
using MayThePerfromanceBeWithYou_Configurator.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
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
    
    private Func<ModSettings> _reloadModSettingsFunction;

    public Func<ModSettings> ReloadModSettingsFunction
    {
        get => _reloadModSettingsFunction;
        set => SetProperty(ref _reloadModSettingsFunction, value);
    }

    public ICommand ReloadModSettingsCommand
    {
        get;
        internal set;
    }

    private void CreateReloadModSettingsCommand()
    {
        ReloadModSettingsCommand = new RelayCommand(Reload, () =>
        {
            return ReloadModSettingsFunction != null;
        });
    }

    private void Reload()
    {
        ModSettings = ReloadModSettingsFunction.Invoke().GetSettingsInfo();
    }

    public void InitializeViewModel(ModSettings modSettings, Func<ModSettings> reloadModSettingsFunction)
    {
        ModSettings = modSettings.GetSettingsInfo();
        ReloadModSettingsFunction = reloadModSettingsFunction;
    }
    
    public ModSettingsViewerViewModel()
    {
        CreateReloadModSettingsCommand();
    }

}