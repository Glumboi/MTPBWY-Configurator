using System;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;
/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : UiPage
{
    private MainPageViewModel _viewModel;
    public MainPageViewModel ViewModel => _viewModel;
    
    public MainPage()
    {
        InitializeComponent();
        _viewModel = (MainPageViewModel)DataContext;
        _viewModel.AssignNotificationBar(ref NotificationBar_SnackBar);
    }
}