using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MayThePerfromanceBeWithYou_Configurator.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

internal class MainPageViewModel : ViewModelBase
{
    private PresetDataBase _database;
    private IniFile _presetIni;

    private Snackbar NotificationBar
    {
        get;
        set;
    }

    private string _gamePath = string.Empty;
    public string GamePath
    {
        get => _gamePath;
        set
        {
            if (value != _gamePath)
            {
                SetProperty(ref _gamePath, value);
            }
        }
    }       
    
    private bool _potatoTextures = false;
    public bool PotatoTextures
    {
        get => _potatoTextures;
        set
        {
            if (value != _potatoTextures)
            {
                SetProperty(ref _potatoTextures, value);
            }
        }
    }         
    
    private bool _disableBloom = false;
    public bool DisableBloom
    {
        get => _disableBloom;
        set
        {
            if (value != _disableBloom)
            {
                SetProperty(ref _disableBloom, value);
            }
        }
    }         
    
    private bool _disableLensFlare = false;
    public bool DisableLensFlare
    {
        get => _disableLensFlare;
        set
        {
            if (value != _disableLensFlare)
            {
                SetProperty(ref _disableLensFlare, value);
            }
        }
    }      
    
    private bool _lqTAA = false;
    public bool LqTAA
    {
        get => _lqTAA;
        set
        {
            if (value != _lqTAA)
            {
                SetProperty(ref _lqTAA, value);
            }
        }
    }

    private int _taaResolution = 70;
    public int TaaResolution
    {
        get => _taaResolution;
        set
        {
            if (value != _taaResolution)
            {
                SetProperty(ref _taaResolution, value);
            }
        }
    }    
    
    private int _selectedPreset = 0;
    public int SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            if (value != _selectedPreset)
            {
                //Update settings UI
                SetProperty(ref _selectedPreset, value);

                UpdateUiFromPreset();
            }
        }
    }

    private void UpdateUiFromPreset(bool useList = true)
    {
        if (useList)
        {
            _presetIni = new IniFile(_iniPresets[_selectedPreset].IniUrl);
        }
                
        TaaResolution = Int32.Parse(_presetIni.Read("r.ScreenPercentage", "SystemSettings"));
                
        LqTAA = ParseInt(_presetIni.Read("r.TemporalAA.Upsampling", "SystemSettings") )> 0;
        PotatoTextures = ParseInt(_presetIni.Read("r.Streaming.AmortizeCPUToGPUCopy", "SystemSettings") )> 0;
    }

    private int ParseInt(string src)
    {
        int rtrn;
        if (Int32.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        return 0;
    }

    private List<Preset> _iniPresets = new List<Preset>();
    public List<Preset> IniPresets
    {
        get => _iniPresets;
        set
        {
            if (value != _iniPresets)
            {
                SetProperty(ref _iniPresets, value);
            }
        }
    }
    
    public ICommand EditIniCommand
    {
        get;
        internal set;
    }

    private void CreateEditIniCommand()
    {
        EditIniCommand = new RelayCommand(EditIni, LocalIniExists);
    }

    private bool LocalIniExists()
    {
        return File.Exists("tempIni.ini");
    }
    
    public void EditIni()
    {
        Process.Start("notepad.exe","tempIni.ini").WaitForExit();

        _presetIni = new IniFile(_presetIni.Path);
        UpdateUiFromPreset(false);
    }
    
    public ICommand UninstallModCommand
    {
        get;
        internal set;
    }

    private void CreateUninstallModCommand()
    {
        UninstallModCommand = new RelayCommand(UninstallMod);
    }

    public void UninstallMod()
    {
        Mod.Uninstall(GamePath);
        ShowNotification("Uninstalled the Mod successfully!", SymbolRegular.BinFull24);
    }
    
    public ICommand InstallModCommand
    {
        get;
        internal set;
    }

    private void CreateInstallModCommand()
    {
        InstallModCommand = new RelayCommand(InstallMod);
    }

    public void InstallMod()
    {
        Mod.Install(
            _presetIni, 
            GamePath, 
            TaaResolution, 
            LqTAA, 
            DisableBloom, 
            DisableLensFlare, 
            PotatoTextures);
        ShowNotification("Installed the Mod successfully!",SymbolRegular.Checkmark48);
    }
    
    public ICommand BrowseFolderCommand
    {
        get;
        internal set;
    }

    private void CreateBrowseFolderCommand()
    {
        BrowseFolderCommand = new RelayCommand(BrowseFolder);
    }

    public void BrowseFolder()
    {
        using (var openDialog = new CommonOpenFileDialog())
        {
            openDialog.IsFolderPicker = true;
            if (openDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                GamePath = openDialog.FileName;
            }
        }
    }

    private void LoadExternalValues()
    {
        GamePath = Core.Game.GetJediSurvivorPath();
    }

    private void InitializeViewModel()
    {
        Task.Run(() =>
        {
            CreateInstallModCommand();
            CreateUninstallModCommand();
            CreateBrowseFolderCommand();
            LoadExternalValues();
            CreateEditIniCommand();
            _database = new PresetDataBase("https://pastebin.com/raw/d0pvppae");
            IniPresets = _database.GetPresets();
            while (_presetIni == null)
            {
                SelectedPreset = 1;
                SelectedPreset = 0;
            }
        });
    }
    
    private void ShowNotification(string content, SymbolRegular icon = SymbolRegular.Info28)
    {
        NotificationBar.Content = content;
        NotificationBar.Icon = icon;

        NotificationBar.Show();
    }

    public void AssignNotificationBar(ref Snackbar notiBar)
    {
        NotificationBar = notiBar;
        notiBar.FontWeight = FontWeights.Bold;
    }
    
    public MainPageViewModel()
    {
        InitializeViewModel();
    }
}