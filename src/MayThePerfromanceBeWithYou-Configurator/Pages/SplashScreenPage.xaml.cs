using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MayThePerfromanceBeWithYou_Configurator.ViewModels;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;

public partial class SplashScreenPage : UiPage
{
    public Storyboard FadeOutAnimation => (Storyboard)TryFindResource("FadeOutAnimation");

    private MainPageViewModel _viewModel;
    public bool IsFadeOutDone
    {
        get;
        private set;
    }

    public SplashScreenPage(ViewModelBase viewModel)
    {
        InitializeComponent();
        FadeOutAnimation.Completed += FadeOutAnimation_Completed;
        ViewModelHelpers.SetViewModel<UiPage>(this, _viewModel);
    }

    private void FadeOutAnimation_Completed(object? sender, System.EventArgs e)
    {
        IsFadeOutDone = true;
    }

    private void SkipButton_OnClick(object sender, RoutedEventArgs e)
    {
        _viewModel.ContentLoaded = true;
    }
}