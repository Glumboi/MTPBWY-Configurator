using System.Windows.Controls;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Pages;

public partial class SplashScreenPage : UiPage
{
    public Storyboard FadeOutAnimation => (Storyboard)TryFindResource("FadeOutAnimation");
    public bool IsFadeOutDone
    {
        get;
        private set;
    }

    public SplashScreenPage()
    {
        InitializeComponent();
        FadeOutAnimation.Completed += FadeOutAnimation_Completed;
    }

    private void FadeOutAnimation_Completed(object? sender, System.EventArgs e)
    {
        IsFadeOutDone = true;
    }
}