using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace MayThePerfromanceBeWithYou_Configurator.CustomControls;

public partial class SettingsSet : UserControl
{
    public SettingsSet()
    {
        InitializeComponent();
    }

    private void SettingsSet_OnLoaded(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i < 10; i++)
        {
            SettingsParent.Children.Add(new Slider());
            SettingsParent.Children.Add(new CheckBox());
        }
        
    }
}