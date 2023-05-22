using System;
using System.Windows;
using System.Windows.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public static class ViewModelHelpers
{
    public static void SetViewModel<T>(T target, ViewModelBase viewModel, Action onInitialization = null)
    {
        if (target is Page)
        {
            var page = target as Page;
            page.DataContext = viewModel;
        }

        if (target is Window)
        {
            var window = target as Window;
            window.DataContext = viewModel;
        }

        if (onInitialization != null) onInitialization.Invoke();
    }
}