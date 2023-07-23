using MayThePerfromanceBeWithYou_Configurator.CustomControls;
using MayThePerfromanceBeWithYou_Configurator.Universal;
using System;
using System.Collections.Generic;
using System.Windows;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;

public partial class ConfigSelectionPage : UiPage
{
    private List<Plugin> _plugins;

    private int _pluginSelection;

    private Window _callerWindow;

    public ConfigSelectionPage(List<Plugin> plugins, int pluginSelection, Window callerWindow)
    {
        InitializeComponent();
        _plugins = plugins;
        _pluginSelection = pluginSelection;
        _callerWindow = callerWindow;
    }

    private void uiPage_Loaded(object sender, RoutedEventArgs e)
    {
        if(_plugins.Count == Games_Panel.Children.Count) return;
        
        for (int i = 0; i < _plugins.Count; i++)
        {
            Plugin plugin = _plugins[i];
            var button = new GameConfigButton(
                150,
                155,
                plugin.GetGameCover(),
                i,
                _callerWindow);
            Games_Panel.Children.Add(button);
        }
    }
}