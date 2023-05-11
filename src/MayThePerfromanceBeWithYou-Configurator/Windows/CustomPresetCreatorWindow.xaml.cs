using System.Windows;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.Pages;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Windows;

public partial class CustomPresetCreatorWindow : UiWindow
{
    private PresetCreatorPage _presetCreatorPage;
    
    public CustomPresetCreatorWindow(IniFile iniFile)
    {
        InitializeComponent();
        _presetCreatorPage = new PresetCreatorPage(iniFile);
        WindowFrame.Navigate(_presetCreatorPage);
    }
}