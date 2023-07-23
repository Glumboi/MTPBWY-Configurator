using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;
/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : UiPage
{
    private readonly MainPageViewModel _viewModel = new MainPageViewModel();
    public MainPageViewModel ViewModel => _viewModel;
    
    public MainPage()
    {
        InitializeComponent();
        ViewModelHelpers.SetViewModel<UiPage>(this, _viewModel, () =>
        {
            _viewModel.AssignNotificationBar(ref NotificationBar_SnackBar);
        });
    }
}