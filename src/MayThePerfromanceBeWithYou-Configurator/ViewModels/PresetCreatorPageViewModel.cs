using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MayThePerfromanceBeWithYou_Configurator.Core;
using Wpf.Ui.Common;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public class PresetCreatorPageViewModel : ViewModelBase
{
    private string _presetName;
    public string PresetName
    {
        get => _presetName;
        set
        {
            SetProperty(ref _presetName, value);
        }
    }    
    
    private IniFile _customIniFile;
    public IniFile CustomIniFile
    {
        get => _customIniFile;
        set
        {
            SetProperty(ref _customIniFile, value);
        }
    }
    
    public ICommand SavePresetCommand
    {
        get;
        internal set;
    }

    private void CreateSavePresetCommand()
    {
        SavePresetCommand = new RelayCommand(SavePreset);         
    }

    public void SavePreset()
    {
        if (string.IsNullOrWhiteSpace(PresetName))
        {
            MessageBox.Show("Name can't be empty!", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        string presetsDirectory = Path.Combine(CustomIniFile.EXE, "CustomPresets");
        
        if (!Directory.Exists(presetsDirectory))
        {
            Directory.CreateDirectory(presetsDirectory);
        }
        
        string customPresetContent = File.ReadAllText(CustomIniFile.Path);
        string destination = Path.Combine(presetsDirectory, PresetName + ".txt");

        if (File.Exists(destination))
        {
            MessageBoxResult msgBox = MessageBox.Show(
                "A Preset with the given Name already exists, do you want to replace it?",
                "Info",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (msgBox == MessageBoxResult.Yes)
            {
                CopyPreset(customPresetContent, destination, true);
            }
            
            return;
        }
        
        CopyPreset(customPresetContent, destination);
    }

    private void CopyPreset(string content, string dest, bool existsAlready = false)
    {
        string customPresetsDataBase = Path.Combine(CustomIniFile.EXE, "CustomPresets.txt");
        Preset newPreset = new Preset(dest, PresetName);
        File.WriteAllText(dest, content);

        if (!File.Exists(customPresetsDataBase))
        {
            File.Create(customPresetsDataBase).Close();
        }

        if (existsAlready)
        {
            return;
        }
        File.AppendAllText(customPresetsDataBase, $"[{newPreset.Name}] Path: {dest},{Environment.NewLine}");
    }
    
    public PresetCreatorPageViewModel()
    {
        CreateSavePresetCommand();
    }
}