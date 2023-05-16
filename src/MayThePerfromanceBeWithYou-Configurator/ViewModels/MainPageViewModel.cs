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
using MayThePerfromanceBeWithYou_Configurator.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly string _saveLocation = System.Environment.GetEnvironmentVariable("USERPROFILE") +
                                           @"\Saved Games\Respawn\JediSurvivor\";

    private PresetDatabase _database;
    private CustomPresetsDatabase _customPresetDatabase;
    private IniFile _presetIni;

    private Snackbar NotificationBar
    {
        get;
        set;
    }

    private bool _contentLoaded = false;

    public bool ContentLoaded
    {
        get => _contentLoaded;
        set => SetProperty(ref _contentLoaded, value);
    }

    private string _gamePath = string.Empty;

    public string GamePath
    {
        get => _gamePath;
        set
        {
            SetProperty(ref _gamePath, value);
            LoadInstallState();
        }
    }

    private string _installationState = string.Empty;

    public string InstallationState
    {
        get => _installationState;
        set => SetProperty(ref _installationState, value);
    }

    private Brush _stateColor;

    public Brush StateColor
    {
        get => _stateColor;
        set => SetProperty(ref _stateColor, value);
    }

    private bool _potatoTextures = false;

    public bool PotatoTextures
    {
        get => _potatoTextures;
        set => SetProperty(ref _potatoTextures, value);
    }

    private bool _disableBloom = false;

    public bool DisableBloom
    {
        get => _disableBloom;
        set => SetProperty(ref _disableBloom, value);
    }

    private bool _disableLensFlare = false;

    public bool DisableLensFlare
    {
        get => _disableLensFlare;
        set => SetProperty(ref _disableLensFlare, value);
    }

    private bool _disableDOF = false;

    public bool DisableDOF
    {
        get => _disableDOF;
        set => SetProperty(ref _disableDOF, value);
    }

    private bool _experimentalStutterFix = false;

    public bool ExperimentalStutterFix
    {
        get => _experimentalStutterFix;
        set => SetProperty(ref _experimentalStutterFix, value);
    }

    private bool _disableAntiAliasing = false;

    public bool DisableAntiAliasing
    {
        get => _disableAntiAliasing;
        set
        {
            if (value)
            {
                TAAGen5 = !value;
                TAAUpscaling = !value;
            }
            SetProperty(ref _disableAntiAliasing, value);
        }
    }

    private bool _limitPoolSizeToVram = false;

    public bool LimitPoolSizeToVram
    {
        get => _limitPoolSizeToVram;
        set => SetProperty(ref _limitPoolSizeToVram, value);
    }

    private bool _disableFog = false;

    public bool DisableFog
    {
        get => _disableFog;
        set => SetProperty(ref _disableFog, value);
    }

    private bool _taaUpscaling = false;

    public bool TAAUpscaling
    {
        get => _taaUpscaling;
        set
        {
            SetProperty(ref _taaUpscaling, value);
            if (value == true)
            {
                TAAGen5 = value;
            }
        }
    }

    private bool _taaGen5 = false;

    public bool TAAGen5
    {
        get => _taaGen5;
        set => SetProperty(ref _taaGen5, value);
    }

    private int _taaResolution = 70;

    public int TaaResolution
    {
        get => _taaResolution;
        set => SetProperty(ref _taaResolution, value);
    }

    private int _toneMapperSharpening = 0;

    public int ToneMapperSharpening
    {
        get => _toneMapperSharpening;
        set => SetProperty(ref _toneMapperSharpening, value);
    }

    private int _viewDistance = 0;

    public int ViewDistance
    {
        get => _viewDistance;
        set => SetProperty(ref _viewDistance, value);
    }

    private List<Preset> _iniPresets = new List<Preset>();

    public List<Preset> IniPresets
    {
        get => _iniPresets;
        set => SetProperty(ref _iniPresets, value);
    }

    private int _selectedPreset = 0;

    public int SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            //Update settings UI
            SetProperty(ref _selectedPreset, value);

            UpdateUiFromPreset();
        }
    }

    private List<PoolSize> _poolSizes = new List<PoolSize>()
    {
        new PoolSize("Above 12 GB", 6144),
        new PoolSize("11 GB - 12 GB", 4069),
        new PoolSize("8 GB - 10 GB", 3072),
        new PoolSize("6 GB", 2048),
        new PoolSize("1 GB - 4 GB", 1024),
    };

    public List<PoolSize> PoolSizes
    {
        get => _poolSizes;
        set => SetProperty(ref _poolSizes, value);
    }

    private int _selectedPoolSize = 0;

    public int SelectedPoolSize
    {
        get => _selectedPoolSize;
        set => SetProperty(ref _selectedPoolSize, value);
    }

    private void SelectProperVramConfig()
    {
        int systemVram = GPU.GetVramInGb();

        switch (systemVram)
        {
            case > 12:
                SelectedPoolSize = 0;
                break;

            case 8:
            case < 8 and > 6:
                SelectedPoolSize = PoolSizes.Count - 3;
                break;

            case 6:
            case < 6 and > 4:
                SelectedPoolSize = PoolSizes.Count - 2;
                break;

            case 4:
            case < 4 and > 0:
                SelectedPoolSize = PoolSizes.Count - 1;
                break;
        }
    }

    private void UpdateUiFromPreset(bool useList = true)
    {
        if (useList)
        {
            _presetIni = new IniFile(_iniPresets[_selectedPreset].IniUrl);
        }

        TaaResolution = LoadSlider(_presetIni.Read("r.ScreenPercentage", "SystemSettings"), 100);
        ToneMapperSharpening = LoadSlider(_presetIni.Read("r.Tonemapper.Sharpen", "SystemSettings"), 0) * 10;
        ViewDistance = LoadSlider(_presetIni.Read("r.ViewDistanceScale", "SystemSettings"), 0);

        TAAUpscaling = MathHelpers.ParseInt(_presetIni.Read("r.TemporalAA.Upsampling", "SystemSettings"), true) != 0;// ;
        TAAGen5 = MathHelpers.ParseInt(_presetIni.Read("r.TemporalAA.Algorithm", "SystemSettings"), true) != 0;// 0;
        PotatoTextures = MathHelpers.ParseInt(_presetIni.Read("r.Streaming.AmortizeCPUToGPUCopy", "SystemSettings")) != 9999;

        DisableLensFlare = MathHelpers.ParseInt(_presetIni.Read("r.LensFlareQuality", "SystemSettings")) == 0;
        DisableBloom = MathHelpers.ParseInt(_presetIni.Read("r.BloomQuality", "SystemSettings")) == 0;
        DisableFog = MathHelpers.ParseInt(_presetIni.Read("r.VolumetricFog", "SystemSettings")) == 0 &&
                     MathHelpers.ParseInt(_presetIni.Read("r.Fog", "SystemSettings")) == 0;
        DisableDOF = MathHelpers.ParseInt(_presetIni.Read("r.DepthOfFieldQuality", "SystemSettings")) == 0;
        ExperimentalStutterFix = MathHelpers.ParseInt(_presetIni.Read("s.ForceGCAfterLevelStreamedOut", "SystemSettings")) == 0;
        DisableAntiAliasing = MathHelpers.ParseInt(_presetIni.Read("r.PostProcessAAQuality", "SystemSettings")) == 0;
        LimitPoolSizeToVram = MathHelpers.ParseInt(_presetIni.Read("r.Streaming.LimitPoolSizeToVRAM", "SystemSettings")) == 1;
    }

    public int LoadSlider(string src, int defaultValue)
    {
        int rtrn = 0;

        if (string.IsNullOrWhiteSpace(src)) return defaultValue;

        if (!MathHelpers.IsFloatOrInt(src)) //If src is a float
        {
            return (int)(MathHelpers.ParseFloat(src));
        }

        if (Int32.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        return defaultValue;
    }

    public ICommand SaveCustomPresetCommand
    {
        get;
        internal set;
    }

    private void CreateSaveCustomPresetCommand()
    {
        SaveCustomPresetCommand = new RelayCommand(CreatePreset);
    }

    public void CreatePreset()
    {
        InstallMod(true, true);
        new CustomPresetCreatorWindow(_presetIni).ShowDialog();
        SelectedPreset = 0;
        Task.Run(InitializePresets);
        ShowNotification("Reinitialized the Databases, if a custom Preset got created it should now be available!");
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

        _presetIni = new IniFile(_presetIni.Path, true);
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

    private bool IsGamePathFilled()
    {
        return !string.IsNullOrWhiteSpace(GamePath);
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

    public ICommand BuildModCommand
    {
        get;
        internal set;
    }

    private void CreateBuildModCommand()
    {
        BuildModCommand = new RelayCommand(param => InstallMod(true));
    }

    public ICommand InstallModCommand
    {
        get;
        internal set;
    }

    private void CreateInstallModCommand()
    {
        InstallModCommand = new RelayCommand(param => InstallMod(false), IsGamePathFilled);
    }

    public void InstallMod(bool buildOnly, bool iniOnly = false)
    {
        Mod.Install(
            buildOnly,
            iniOnly,
            _presetIni,
            PoolSizes[SelectedPoolSize],
            GamePath,
            TaaResolution,
            TAAUpscaling,
            TAAGen5,
            DisableBloom,
            DisableLensFlare,
            PotatoTextures,
            ToneMapperSharpening,
            DisableDOF,
            DisableFog,
            ViewDistance,
            ExperimentalStutterFix,
            DisableAntiAliasing,
            LimitPoolSizeToVram);

        if (iniOnly) return;
        if (buildOnly)
        {
            ShowNotification("Mod built successfully!", SymbolRegular.Wrench24);
            return;
        }
        LoadInstallState();
        ShowNotification("Installed the Mod successfully!", SymbolRegular.Checkmark48);
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
            _database = new PresetDatabase("https://gistcdn.githack.com/Glumboi/074c19fabc18efc2b2df28009d91a036/raw/MTPBWY-Presets.txt");
            IniPresets = _database.GetPresets();
            _database.CreateLocalDatabase();
        }
        catch
        {
            _database = new PresetDatabase(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LocalDatabase.txt"));
            IniPresets = _database.GetPresets();
        }

        _customPresetDatabase = new CustomPresetsDatabase(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CustomPresets.txt"));
        IniPresets.AddRange(_customPresetDatabase.GetPresets());
    }

    private void InitializeViewModel()
    {
        Task.Run(() =>
        {
            CreateSaveCustomPresetCommand();
            CreateInstallModCommand();
            CreateUninstallModCommand();
            CreateBrowseFolderCommand();
            CreateBrowseSaveCommandCommand();
            CreateEditIniCommand();
            CreateBuildModCommand();
            LoadExternalValues();
            SelectProperVramConfig();

            do
            {
                InitializePresets();
            } while (_customPresetDatabase == null || _database == null);

            do
            {
                SelectedPreset = 1;
                SelectedPreset = 0;
            } while (_presetIni == null);

            ContentLoaded = true;
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
        notiBar.Timeout = 5000;
    }

    public MainPageViewModel()
    {
        InitializeViewModel();
    }
}