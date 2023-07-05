using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MayThePerfromanceBeWithYou_Configurator.Windows;

namespace MayThePerfromanceBeWithYou_Configurator;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.ToString());
        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
