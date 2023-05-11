using System;
using System.Collections.Generic;
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
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : UiWindow
{
    private MainPage mainPage;
    private readonly SplashScreenPage splashPage = new SplashScreenPage();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void CheckLoadCompleted()
    {
        mainPage = new MainPage();

        Task.Run(() =>
        {
            while (!mainPage.ViewModel.ContentLoaded)
            {
                continue;
            }

            splashPage.Dispatcher.Invoke(() =>
            {
                splashPage.FadeOutAnimation.Begin();
            });

            while (!splashPage.IsFadeOutDone)
            {
                continue;
            }

            WindowFrame.Dispatcher.Invoke(() =>
            {
                WindowFrame.Navigate(mainPage);
            });
        });
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        WindowFrame.Navigate(splashPage);
        CheckLoadCompleted();
    }
}