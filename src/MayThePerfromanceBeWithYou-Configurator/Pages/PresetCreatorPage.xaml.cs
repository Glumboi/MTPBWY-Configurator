using System.Windows.Controls;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;

public partial class PresetCreatorPage : UiPage
{
    private PresetCreatorPageViewModel _viewModel = new PresetCreatorPageViewModel();
    
    public PresetCreatorPage(IniFile iniFile)
    {
        InitializeComponent();
        ViewModelHelpers.SetViewModel<UiPage>(this, _viewModel, () =>
        {
            _viewModel.TempIniFile = iniFile;
        });
    }
}