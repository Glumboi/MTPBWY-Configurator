using System;
using System.Windows;
using System.Windows.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public static class ViewModelHelpers
{
    public static void SetViewModel<T>(T target, ViewModelBase viewModel, Action onInitialization = null)
    {
        switch (target)
        {
            case Page page1:
            {
                var page = page1;
                page.DataContext = viewModel;
                break;
            }
            case Window window1:
            {
                var window = window1;
                window.DataContext = viewModel;
                break;
            }
            default:
                return;
        }

        if (onInitialization != null) onInitialization.Invoke();
    }
}