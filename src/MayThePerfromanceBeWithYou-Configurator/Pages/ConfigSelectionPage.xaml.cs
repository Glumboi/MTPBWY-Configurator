using MayThePerfromanceBeWithYou_Configurator.CustomControls;
using MayThePerfromanceBeWithYou_Configurator.Universal;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MayThePerfromanceBeWithYou_Configurator.Constants;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;

public partial class ConfigSelectionPage : UiPage
{
    private int _pluginSelection;

    private Window _callerWindow;

    public ConfigSelectionPage(int pluginSelection, Window callerWindow)
    {
        InitializeComponent();
        _pluginSelection = pluginSelection;
        _callerWindow = callerWindow;
    }

    private void uiPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (ConstantInstances._pluginList.Count == Games_Panel.Children.Count) return;

        for (int i = 0; i < ConstantInstances._pluginList.Count; i++)
        {
            Plugin plugin = ConstantInstances._pluginList[i];
            var button = new GameConfigButton(
                150,
                155,
                plugin.GetGameCover(),
                i);
            Games_Panel.Children.Add(button);
            button.Click += (o, args) =>
            {
                var mw = _callerWindow as MainWindow;
                mw.mainPage.ViewModel.SelectedPlugin = button.PluginIndex;
                mw.NavigateToPage(mw.mainPage);
            };
        }
    }

    private void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        for (int i = 0; i < Games_Panel.Children.Count; i++)
        {
            var button = Games_Panel.Children[i] as GameConfigButton;
            if (button.GameName.ToLower().Contains(FilterTextBox.Text.ToLower()))
            {
                button.Visibility = Visibility.Visible;
                continue;
            }

            button.Visibility = Visibility.Collapsed;
        }
    }
}