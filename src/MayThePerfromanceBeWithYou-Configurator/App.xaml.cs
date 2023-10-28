using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
        string errorMessage = string.Format("\nAn unhandled exception occurred: {0}", e.Exception.InnerException);
        AppLogging.SayToLogFile(errorMessage, AppLogging.LogFileMsgType.ERROR);
        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        string logFile = "MTPBWY-C.log";
        
        if (File.Exists(logFile) && new FileInfo(logFile).Length > 5242880)
        {
             File.Delete(logFile); //If file gets bigger than 5.2 MB, delete it
        }

        Trace.Listeners.Add(new TextWriterTraceListener(logFile));
        Trace.AutoFlush = true;
        AppLogging.SayToLogFile("Logging initialized!", AppLogging.LogFileMsgType.INFO);

        base.OnStartup(e);
    }
}

public static class AppLogging
{
    public enum LogFileMsgType
    {
        UNDEFINED,
        INFO,
        WARNING,
        ERROR,
        DEBUG
    }

    public static void SayToLogFile(string message, LogFileMsgType type, string pluginIdentifier = null)
    {
        DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        string identifierText = pluginIdentifier == null ? "Configurator" : pluginIdentifier;
        string compiledMessage = $"[From {identifierText} | {type} | {formattedDateTime}]: {message}";
        Trace.WriteLine(compiledMessage);
        Trace.Flush();
    }
}