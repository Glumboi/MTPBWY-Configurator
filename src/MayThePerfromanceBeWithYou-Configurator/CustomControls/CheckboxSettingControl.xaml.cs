using System.Windows.Controls;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.CustomControls;

public partial class CheckboxSettingControl : CheckBox
{
    public CheckboxSettingControl()
    {
        InitializeComponent();
        SettingsControlViewModel viewModel = new SettingsControlViewModel();
        ViewModelHelpers.SetViewModel<CheckBox>(this, viewModel);
    }
}