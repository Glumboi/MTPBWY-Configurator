using System;
using System.Windows;
using System.Windows.Controls;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.Pages;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Windows;

public partial class ModSettingsViewerWindow : UiWindow
{
    private readonly ModSettingsViewerPage _page = new ModSettingsViewerPage();
    public ModSettingsViewerWindow(ModSettings modSettings, Func<ModSettings> reloadModSettingsFunction)
    {
        InitializeComponent();
        ModSettingsViewerViewModel viewModel = _page.DataContext as ModSettingsViewerViewModel;
        ViewModelHelpers.SetViewModel<UiWindow>(this, viewModel, () =>
        {
            viewModel.InitializeViewModel(modSettings, reloadModSettingsFunction);
        });
        WindowFrame.Navigate(_page);
    }
}