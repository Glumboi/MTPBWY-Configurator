using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using MayThePerfromanceBeWithYou_Configurator.Core;
using MayThePerfromanceBeWithYou_Configurator.CustomSettings;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public class SettingsControlViewModel : ViewModelBase
{
    private List<CustomSetting> _settings = new();

    public List<CustomSetting> Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    private ObservableCollection<Control> _checkBoxSettings = new();

    public ObservableCollection<Control> CheckBoxSettings
    {
        get => _checkBoxSettings;
        set => SetProperty(ref _checkBoxSettings, value);
    }


    private ObservableCollection<Control> _sliderSettings = new();

    public ObservableCollection<Control> SliderSettings
    {
        get => _sliderSettings;
        set => SetProperty(ref _sliderSettings, value);
    }

    private ObservableCollection<Control> _textboxSetting = new();

    public ObservableCollection<Control> TextboxSettings
    {
        get => _textboxSetting;
        set => SetProperty(ref _textboxSetting, value);
    }

    public void RealoadSettings()
    {
        Settings.Clear();
        SliderSettings.Clear();
        CheckBoxSettings.Clear();
        TextboxSettings.Clear();

        foreach (var file in Directory.GetFiles("CustomSettingsConfigs"))
        {
            var setting = new CustomSetting(file);
            switch (setting.JsonData.SettingType)
            {
                case CustomSettingType.CHECKBOX:
                    var cb = new CheckBox()
                    {
                        Content = setting.JsonData.SettingName,
                        Margin = new Thickness(8, 0, 0, 0)
                    };

                    cb.Checked += (sender, args) =>
                    {
                        if (setting.JsonData.DependsOn != "NODEPENDS")
                        {
                            foreach (var control in CheckBoxSettings)
                            {
                                var cbC = (CheckBox)control;
                                if ((string)cbC.Content == setting.JsonData.DependsOn)
                                {
                                    cbC.IsChecked = true;
                                }
                            }
                        }

                        setting.JsonData.DefaultValue = cb.IsChecked == true ? 1.ToString() : 0.ToString();
                    };

                    cb.Unchecked += (sender, args) =>
                    {
                        setting.JsonData.DefaultValue = cb.IsChecked == true ? 1.ToString() : 0.ToString();
                    };
                    CheckBoxSettings.Add(cb);
                    break;
                case CustomSettingType.RANGE:
                    var sl = new Slider
                    {
                        Margin = new Thickness(4, 4, 0, 4),
                        Maximum = Int32.Parse(setting.JsonData.MaxValue),
                        Value = Int32.Parse(setting.JsonData.DefaultValue),
                        Minimum = Int32.Parse(setting.JsonData.MinValue)
                    };

                    sl.ValueChanged += (sender, args) => { setting.JsonData.DefaultValue = sl.Value.ToString("0"); };

                    var slTextBoxValueBinding = new Binding
                    {
                        Source = sl,
                        Path = new PropertyPath("Value")
                    };

                    Binding sliderValueBinding = new Binding("DefaultValue");
                    sliderValueBinding.Source = setting;
                    sliderValueBinding.Mode =
                        BindingMode.TwoWay; // If you want changes in the slider to update DefaultValue
                    BindingOperations.SetBinding(sl, RangeBase.ValueProperty, sliderValueBinding);

                    var tbVal = new TextBlock()
                    {
                        Text = "(0)",
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(4, 5, 0, 0)
                    };

                    slTextBoxValueBinding.StringFormat = $"{setting.JsonData.SettingName}: {0:N0}";
                    tbVal.SetBinding(TextBlock.TextProperty, slTextBoxValueBinding);

                    var gridLower = new Grid()
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(),
                            new ColumnDefinition()
                            {
                                Width = GridLength.Auto,
                            },
                        },

                        Children =
                        {
                            sl,
                            tbVal
                        },
                    };

                    Grid.SetColumn(sl, 0);
                    Grid.SetColumn(tbVal, 1);

                    var grid = new Grid()
                    {
                        RowDefinitions =
                        {
                            new RowDefinition()
                        },

                        Children =
                        {
                            gridLower
                        }
                    };
                    Grid.SetRow(gridLower, 0);
                    SliderSettings.Add(new UserControl()
                    {
                        Content = grid
                    });
                    break;
                case CustomSettingType.UNDEFINED:
                    var undefinedTb = new TextBox()
                    {
                        Text = setting.JsonData.DefaultValue,
                        PlaceholderText = setting.JsonData.SettingName
                    };

                    undefinedTb.TextChanged += (sender, args) => { setting.JsonData.DefaultValue = undefinedTb.Text; };
                    TextboxSettings.Add(undefinedTb);
                    //                           
                    break;
                default:
                    throw new Exception(
                        $"The setting with the path: {file} does not specify a control type!\nPlease ensure that the setting is valid or delete it!");
            }

      

            Settings.Add(setting);
        }
    }

    public SettingsControlViewModel()
    {
        RealoadSettings();
    }
}