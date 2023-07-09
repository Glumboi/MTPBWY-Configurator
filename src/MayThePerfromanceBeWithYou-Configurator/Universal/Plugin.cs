using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MayThePerfromanceBeWithYou_Configurator.Core;

namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public class Plugin : IPlugin
{
    public string DisplayName => _pluginNamespace;
    
    private IniFile _pluginIni;
    private string _pluginDll;
    private string _pluginClass;
    private string _pluginNamespace;
    private object[] _pluginParams;
    Assembly assembly;
    private Type _funcType;
    public Action<bool, bool, IniFile, PoolSize, string, ModSettings> _installFunc;
    public Action<string> _uninstallFunc;
    public Func<string, bool> _installedFunc;

    public Plugin(string ini)
    {
        _pluginIni = new IniFile(ini, true);
        _pluginClass = _pluginIni.Read("Class", "Plugin");
        _pluginNamespace = _pluginIni.Read("Namespace", "Plugin");
        _pluginDll = Path.Combine(_pluginIni.EXE, _pluginIni.Read("DllPath", "Plugin"));
        assembly = Assembly.LoadFile(_pluginDll);
        _funcType = assembly.GetType($"{_pluginNamespace}.{_pluginClass}");
        LoadParams();
        FireEntryPoint();
        LoadCommonFunctions();
    }
    
    private void LoadParams()
    {
        string[] paramsOfIni = _pluginIni.Read("Params", "Plugin").Split(',');

        List<object> objects = new List<object>();

        foreach (object parameter in paramsOfIni)
        {
            objects.Add(parameter);
        }

        _pluginParams = objects.ToArray();
    }

    private (MethodInfo, object) GetMethodFromPlugin(string name)
    {
        Type type = _funcType;
        object obj = Activator.CreateInstance(type);
        return (type.GetMethod("EntryPoint"), obj);
    }
    
    public void FireEntryPoint()
    {
        var entryPoint = GetMethodFromPlugin("EntryPoint");
        entryPoint.Item1.Invoke(entryPoint.Item2, new object[] { _pluginParams });
    }

    private void LoadCommonFunctions()
    {
        Type pluginType = typeof(Plugin);

        // Get the Install method
        MethodInfo installMethod = pluginType.GetMethod("Install");
        _installFunc = (Action<bool, bool, IniFile, PoolSize, string, ModSettings>)Delegate.CreateDelegate(
            typeof(Action<bool, bool, IniFile, PoolSize, string, ModSettings>), this, installMethod);

        // Get the IsModInstalled method
        MethodInfo isModInstalledMethod = pluginType.GetMethod("IsModInstalled");
        _installedFunc = (Func<string, bool>)Delegate.CreateDelegate(
            typeof(Func<string, bool>), this, isModInstalledMethod);

        // Get the Uninstall method
        MethodInfo uninstallMethod = pluginType.GetMethod("Uninstall");
        _uninstallFunc = (Action<string>)Delegate.CreateDelegate(
            typeof(Action<string>), this, uninstallMethod);
    }


    public void Install(
        bool buildOnly,
        bool iniOnly,
        IniFile tempIni,
        PoolSize poolSize,
        string gameDir,
        ModSettings modSettings)
    {
        _installFunc.Invoke(buildOnly, iniOnly, tempIni, poolSize, gameDir, modSettings);
    }

    public bool IsModInstalled(string gameDir)
    {
        return _installedFunc.Invoke(gameDir);
    }

    public void Uninstall(string gameDir)
    {
        _uninstallFunc.Invoke(gameDir);
    }
}