using System;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;
/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : UiPage
{
    private MainPageViewModel viewModel;

    public MainPage()
    {
        InitializeComponent();
        viewModel = (MainPageViewModel)DataContext;
        viewModel.AssignNotificationBar(ref NotificationBar_SnackBar);
    }
}