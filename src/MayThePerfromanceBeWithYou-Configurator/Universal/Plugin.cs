using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MayThePerfromanceBeWithYou_Configurator.Core;

namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public class Plugin : IPlugin
{
    public string DisplayName => _pluginNamespace;

    private IniFile _pluginIni;
    private string _pluginDll;
    private string _pluginClass;
    private string _pluginNamespace;
    private string _coverUrl;
    private object[] _pluginParams;
    private Assembly _assembly;

    public Plugin(string ini)
    {
        _pluginIni = new IniFile(ini, true);
        _pluginClass = _pluginIni.Read("Class", "Plugin");
        _pluginNamespace = _pluginIni.Read("Namespace", "Plugin");
        _coverUrl = _pluginIni.Read("GameCover", "Plugin");
        _pluginDll = Path.Combine(_pluginIni.EXE, _pluginIni.Read("DllPath", "Plugin"));
        _assembly = Assembly.LoadFile(_pluginDll);
        LoadParams();
    }

    private void LoadParams()
    {
        string paramsString = _pluginIni.Read("Params", "Plugin");
        _pluginParams = paramsString.Split(',');
    }

    public void FireEntryPoint()
    {
        var entryPointMethod = _assembly.GetType($"{_pluginNamespace}.{_pluginClass}")?.GetMethod("EntryPoint");
        if (entryPointMethod != null)
        {
            object instance = Activator.CreateInstance(entryPointMethod.DeclaringType);
            entryPointMethod.Invoke(instance, new object[] { _pluginParams });
        }
    }

    private Type GetFunctionType()
    {
        return _assembly.GetType($"{_pluginNamespace}.{_pluginClass}");
    }

    private (MethodInfo, object) GetFunctionFromPlugin(string name)
    {
        Type functionType = GetFunctionType();
        object obj = Activator.CreateInstance(functionType);
        MethodInfo methodInfo = functionType.GetMethod(name);
        return (methodInfo, obj);
    }
    
    public void Install(
        bool buildOnly,
        bool iniOnly,
        IniFile tempIni,
        PoolSize poolSize,
        string gameDir,
        ModSettings modSettings)
    {
        var parameters = new List<object> { buildOnly, iniOnly, tempIni, poolSize, gameDir, modSettings };
        var func = GetFunctionFromPlugin("Install");
        func.Item1.Invoke(func.Item2, parameters.ToArray());
    }

    public bool IsModInstalled(string gameDir)
    {
        var func = GetFunctionFromPlugin("IsModInstalled");
        return (bool)func.Item1.Invoke(func.Item2, new object[] { gameDir });
    }

    public void Uninstall(string gameDir)
    {
        var func = GetFunctionFromPlugin("Uninstall");
        func.Item1.Invoke(func.Item2, new object[] { gameDir });
    }

    public string GetGamePath()
    {
        var func = GetFunctionFromPlugin("GetGamePath");
        try
        {
            return (string)func.Item1.Invoke(func.Item2, new object[] { });
        }
        catch
        {
            return string.Empty;
        }
    }

    public string GetGameCover()
    {
        return _coverUrl;
    }

    public void LaunchGame()
    {
        var func = GetFunctionFromPlugin("LaunchGame");
        func.Item1.Invoke(func.Item2, new object[] { });
    }

    public void OpenGameSaveLocation()
    {
        var func = GetFunctionFromPlugin("OpenGameSaveLocation");
        func.Item1.Invoke(func.Item2, new object[] { });
    }

    public bool DoesSaveDirectoryExist()
    {
        var func = GetFunctionFromPlugin("DoesSaveDirectoryExist");
        return (bool)func.Item1.Invoke(func.Item2, new object[] { });
    }

    public static List<Plugin> GetPlugins()
    {
        List<Plugin> rtn = new List<Plugin>();
        var dirs = Directory.GetDirectories(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins"));
        foreach (var dir in dirs)
        {
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                if (file.EndsWith(".ini"))
                {
                    rtn.Add(new Plugin(file));
                }
            }
        }

        return rtn;
    }
}