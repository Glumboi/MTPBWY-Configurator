using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public static class ModSettings
{
    public static List<ModSetting> AllModSettings { get; set; }

    public static List<Wpf.Ui.Controls.CardExpander> CardExpanders { get; set; }

    public static void Load(string xmlLocation)
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlLocation);
            List<ModSetting> modSettingsList = new List<ModSetting>();
            XmlNodeList systemSettingsNodes = xmlDoc.SelectNodes("//ModSettings/*");

            foreach (XmlNode systemSettingsNode in systemSettingsNodes)
            {
                string root = systemSettingsNode.Name;

                if (systemSettingsNode.Name == "Settings" || systemSettingsNode.Name == "BigSetting")
                {
                    string friendlyName = systemSettingsNode.Attributes["FriendlyName"].Value;

                    if (systemSettingsNode.Name == "BigSetting")
                    {
                        string defaultVal = systemSettingsNode.Attributes["defaultVal"].Value;
                        modSettingsList.Add(new ModSetting
                        {
                            Root = root,
                            FriendlyName = friendlyName,
                            Key = null,
                            DefaultValue = defaultVal,
                            ControlType = "CheckBox"
                        });
                    }
                    else if (systemSettingsNode.Name == "Settings")
                    {
                        List<UIElement> childControls = new List<UIElement>();
                        XmlNodeList settingNodes = systemSettingsNode.SelectNodes("Setting");
                        foreach (XmlNode settingNode in settingNodes)
                        {
                            string key = settingNode.Attributes["key"].Value;
                            string defaultVal = settingNode.Attributes["defaultVal"].Value == "NULL"
                                ? null
                                : settingNode.Attributes["defaultVal"].Value;
                            string controlType = settingNode.Attributes["ControlType"].Value;
                            string friendlyNameOfChildSetting = settingNode.Attributes["FriendlyName"].Value;

                            SettingControlTypes controlTypeEnumVal =
                                (SettingControlTypes)Enum.Parse(typeof(SettingControlTypes), controlType, true);

                            switch (controlTypeEnumVal)
                            {
                                case SettingControlTypes.Checkbox:
                                    bool defaultCheckboxValue = bool.Parse(defaultVal);

                                    // Create the CheckBox control
                                    CheckBox checkBox = new CheckBox
                                    {
                                        IsChecked = defaultCheckboxValue,
                                        Content = friendlyNameOfChildSetting,
                                    };

                                    // Create the TextBlock control
                                    TextBlock textBlockCb = new TextBlock
                                    {
                                        Text = friendlyNameOfChildSetting,
                                        FontWeight = FontWeights.Bold,
                                    };

                                    // Create the Grid and add the Slider and TextBlock to it
                                    Grid gridCb = new Grid();
                                    gridCb.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                                    gridCb.Children.Add(checkBox);
                                    gridCb.Children.Add(textBlockCb);

                                    // Set the column indices for the controls
                                    Grid.SetColumn(checkBox, 0);
                                    Grid.SetColumn(textBlockCb, 1);

                                    // Create the UserControl and set its Content to the Grid
                                    UserControl userControlCb = new UserControl
                                    {
                                        Content = gridCb,
                                    };

                                    // Add the UserControl to the childControls list
                                    childControls.Add(userControlCb);
                                    break;

                                case SettingControlTypes.Slider:
                                    double defaultSliderValue = double.Parse(defaultVal);

                                    // Create the Slider control
                                    Slider slider = new Slider
                                    {
                                        Value = defaultSliderValue,
                                    };

                                    // Create the TextBlock control
                                    TextBlock textBlock = new TextBlock
                                    {
                                        Text = friendlyNameOfChildSetting,
                                        FontWeight = FontWeights.Bold,
                                    };

                                    // Create the Grid and add the Slider and TextBlock to it
                                    Grid grid = new Grid();
                                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                                    grid.Children.Add(slider);
                                    grid.Children.Add(textBlock);

                                    // Set the column indices for the controls
                                    Grid.SetColumn(slider, 0);
                                    Grid.SetColumn(textBlock, 1);

                                    // Create the UserControl and set its Content to the Grid
                                    UserControl userControl = new UserControl
                                    {
                                        Content = grid,
                                    };

                                    // Add the UserControl to the childControls list
                                    childControls.Add(userControl);
                                    break;

                                case SettingControlTypes.TextBox:
                                    childControls.Add(new Wpf.Ui.Controls.TextBox()
                                    {
                                        Text = defaultVal,
                                        PlaceholderText = friendlyName + " value"
                                    });
                                    break;

                                //Undefined
                                default:
                                    break;
                            }

                            modSettingsList.Add(new ModSetting
                            {
                                Root = root,
                                FriendlyName = friendlyName,
                                Key = key,
                                DefaultValue = defaultVal,
                                ControlType = controlType
                            });
                        }

                        var wrpPanel = new WrapPanel();
                        for (int i = 0; i < childControls.Count; i++)
                        {
                            wrpPanel.Children.Add(childControls[i]);
                        }

                        CardExpanders.Add(new CardExpander
                        {
                            Header = systemSettingsNode.Attributes["FriendlyName"],
                            Content = wrpPanel
                        });
                    }
                }
            }

            // Display the saved mod settings
            foreach (var modSetting in modSettingsList)
            {
                Debug.WriteLine(
                    $"Root: {modSetting.Root}, FriendlyName: {modSetting.FriendlyName}, Key: {modSetting.Key}, Default Value: {modSetting.DefaultValue}, Control Type: {modSetting.ControlType}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
        }
    }
}

public enum SettingControlTypes
{
    UNDEFINED,
    Checkbox,
    TextBox,
    Slider
}

public class ModSetting
{
    public string FriendlyName { get; set; }
    public string Key { get; set; }
    public string DefaultValue { get; set; }
    public string ControlType { get; set; }
    public string Root { get; set; }

    public ModSetting(string friendlyName, string key, string defaultValue, string controlType, string root)
    {
        FriendlyName = friendlyName;
        Key = key;
        DefaultValue = defaultValue;
        ControlType = controlType;
        Root = root;
    }

    public ModSetting()
    {
    }
}