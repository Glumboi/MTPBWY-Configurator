using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MayThePerfromanceBeWithYou_Configurator.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

internal class MainPageViewModel : ViewModelBase
{
    private readonly string _saveLocation = System.Environment.GetEnvironmentVariable("USERPROFILE") +
                                           @"\Saved Games\Respawn\JediSurvivor\";

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
                LoadInstallState();
            }
        }
    }

    private string _installationState = string.Empty;

    public string InstallationState
    {
        get => _installationState;
        set
        {
            if (value != _installationState)
            {
                SetProperty(ref _installationState, value);
            }
        }
    }

    private Brush _stateColor;

    public Brush StateColor
    {
        get => _stateColor;
        set
        {
            if (value != _stateColor)
            {
                SetProperty(ref _stateColor, value);
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

    private bool _disableDOF = false;

    public bool DisableDOF
    {
        get => _disableDOF;
        set
        {
            if (value != _disableDOF)
            {
                SetProperty(ref _disableDOF, value);
            }
        }
    }

    private bool _experimentalStutterFix = false;

    public bool ExperimentalStutterFix
    {
        get => _experimentalStutterFix;
        set
        {
            if (value != _experimentalStutterFix)
            {
                SetProperty(ref _experimentalStutterFix, value);
            }
        }
    }

    private bool _disableFog = false;

    public bool DisableFog
    {
        get => _disableFog;
        set
        {
            if (value != _disableFog)
            {
                SetProperty(ref _disableFog, value);
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

    private int _toneMapperSharpening = 0;

    public int ToneMapperSharpening
    {
        get => _toneMapperSharpening;
        set
        {
            if (value != _toneMapperSharpening)
            {
                SetProperty(ref _toneMapperSharpening, value);
            }
        }
    }

    private int _viewDistance = 0;

    public int ViewDistance
    {
        get => _viewDistance;
        set
        {
            if (value != _viewDistance)
            {
                SetProperty(ref _viewDistance, value);
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

        TaaResolution = LoadSlider(_presetIni.Read("r.ScreenPercentage", "SystemSettings"));
        ToneMapperSharpening = LoadSlider(_presetIni.Read("r.Tonemapper.Sharpen", "SystemSettings")) * 10;
        ViewDistance = LoadSlider(_presetIni.Read("r.ViewDistanceScale", "SystemSettings"));

        LqTAA = ParseInt(_presetIni.Read("r.TemporalAA.Upsampling", "SystemSettings")) != 9999;// 0;
        PotatoTextures = ParseInt(_presetIni.Read("r.Streaming.AmortizeCPUToGPUCopy", "SystemSettings")) != 9999;

        DisableLensFlare = ParseInt(_presetIni.Read("r.LensFlareQuality", "SystemSettings")) == 0;
        DisableBloom = ParseInt(_presetIni.Read("r.BloomQuality", "SystemSettings")) == 0;
        DisableFog = ParseInt(_presetIni.Read("r.VolumetricFog", "SystemSettings")) == 0 &&
                     ParseInt(_presetIni.Read("r.Fog", "SystemSettings")) == 0;
        DisableDOF = ParseInt(_presetIni.Read("r.DepthOfFieldQuality", "SystemSettings")) == 0;
        ExperimentalStutterFix = ParseInt(_presetIni.Read("s.ForceGCAfterLevelStreamedOut", "SystemSettings")) == 0;
    }

    public bool IsFloatOrInt(string value)
    {
        int intValue;
        float floatValue;
        return Int32.TryParse(value, out intValue);
    }

    public int LoadSlider(string src)
    {
        int rtrn;

        if (!IsFloatOrInt(src)) //If src is a float
        {
            return (int)(ParseFloat(src));
        }

        if (Int32.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        return 0;
    }

    private float ParseFloat(string src)
    {
        float rtrn;
        if (float.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        return 0f;
    }

    private int ParseInt(string src)
    {
        int rtrn;
        if (Int32.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        return 9999;
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
        Process.Start("notepad.exe", "tempIni.ini").WaitForExit();

        _presetIni = new IniFile(_presetIni.Path);
        UpdateUiFromPreset(false);
    }

    public ICommand BrowseSaveCommand
    {
        get;
        internal set;
    }

    private bool DoesSaveDirectoryExist()
    {
        return Directory.Exists(_saveLocation);
    }

    private void CreateBrowseSaveCommandCommand()
    {
        BrowseSaveCommand = new RelayCommand(BrowseSaves, DoesSaveDirectoryExist);
    }

    private void BrowseSaves()
    {
        Process.Start("explorer.exe", _saveLocation);
    }

    public ICommand UninstallModCommand
    {
        get;
        internal set;
    }

    private bool IsModAlreadyInstalled()
    {
        return Mod.IsModInstalled(GamePath);
    }

    private void CreateUninstallModCommand()
    {
        UninstallModCommand = new RelayCommand(UninstallMod, IsModAlreadyInstalled);
    }

    public void UninstallMod()
    {
        Mod.Uninstall(GamePath);
        ShowNotification("Uninstalled the Mod successfully!", SymbolRegular.BinFull24);
        LoadInstallState();
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
            PotatoTextures,
            ToneMapperSharpening,
            DisableDOF,
            DisableFog,
            ViewDistance,
            ExperimentalStutterFix);
        ShowNotification("Installed the Mod successfully!", SymbolRegular.Checkmark48);
        LoadInstallState();
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

    private void LoadInstallState()
    {
        if (IsModAlreadyInstalled())
        {
            InstallationState = "Installed";
            StateColor = Brushes.LightGreen;
            return;
        }

        InstallationState = "Not Installed";
        StateColor = Brushes.IndianRed;
    }

    private void InitializePresets()
    {
        try
        {
            _database = new PresetDataBase("https://gistcdn.githack.com/Glumboi/074c19fabc18efc2b2df28009d91a036/raw/MTPBWY-Presets.txt");
            IniPresets = _database.GetPresets();
            _database.CreateLocalDatabase();
        }
        catch
        {
            _database = new PresetDataBase(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LocalDatabase.txt"));
            IniPresets = _database.GetPresets();
        }
    }

    private void InitializeViewModel()
    {
        Task.Run(() =>
        {
            CreateInstallModCommand();
            CreateUninstallModCommand();
            CreateBrowseFolderCommand();
            CreateBrowseSaveCommandCommand();
            CreateEditIniCommand();
            LoadExternalValues();
            InitializePresets();

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