using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
}