﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using MayThePerfromanceBeWithYou_Configurator.Pages;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : UiWindow
{
    public MainPage mainPage = new MainPage();
    public ConfigSelectionPage configPage;
    public SplashScreenPage splashPage;

    public MainWindow()
    {
        DataContext = mainPage.DataContext;
        InitializeComponent();
        Accent.ApplySystemAccent();
    }

    private async void CheckLoadCompleted()
    {
        var tcs = new TaskCompletionSource<bool>();

        // Poll the ContentLoaded property until it becomes true
        while (!mainPage.ViewModel.ContentLoaded)
        {
            await Task.Delay(100);
        }

        // Start the fade-out animation
        splashPage.Dispatcher.Invoke(() => splashPage.FadeOutAnimation.Begin());

        // Wait for the fade-out animation to complete
        tcs = new TaskCompletionSource<bool>();
        splashPage.FadeOutAnimation.Completed += (sender, e) => tcs.SetResult(true);
        await tcs.Task;

        configPage = new ConfigSelectionPage(
            mainPage.ViewModel.Plugins,
            mainPage.ViewModel.SelectedPlugin,
            this);
        
        NavigateToPage(configPage);
        BackButton.Visibility = Visibility.Visible;
    }

    public void NavigateToPage(UiPage page)
    {
        WindowFrame.Dispatcher.Invoke(() => WindowFrame.Navigate(page));
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        splashPage = new SplashScreenPage((ViewModelBase)DataContext);
        NavigateToPage(splashPage);
        CheckLoadCompleted();
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        WindowFrame.Navigate(configPage);
    }

    private void WindowFrame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode == NavigationMode.Back)
        {
            e.Cancel = true;
        }
    }
}