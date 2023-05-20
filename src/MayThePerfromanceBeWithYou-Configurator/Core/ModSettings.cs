using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Documents;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public struct ModSettings
{
    public TAASettings TaaSettings
    {
        get;
        set;
    }

    public bool DisableBloom    
    {
        get;
        set;
    }
    
    public bool DisableLensFlare    
    {
        get;
        set;
    }
    
    public bool PotatoTextures    
    {
        get;
        set;
    }
    
    public int ToneMapperSharpening    
    {
        get;
        set;
    }
    
    public bool DisableDof    
    {
        get;
        set;
    }
    
    public bool DisableFog    
    {
        get;
        set;
    }
    
    public int ViewDistance   
    {
        get;
        set;
    }
    
    public bool UseExperimentalStutterFix    
    {
        get; 
        set;
    }
    
    public bool DisableAntiAliasing    
    {
        get; 
        set;
    }
    
    public bool EnablePoolSizeToVramLimit    
    {
        get;
        set;
    }

    private PropertyInfo[] _properties;
    private PropertyInfo[] _propertiesOfTAASettings;
    
    public ModSettings(
        bool enablePoolSizeToVramLimit, 
        bool disableAntiAliasing, 
        bool useExperimentalStutterFix, 
        int viewDistance, 
        bool disableFog, 
        int toneMapperSharpening,
        bool potatoTextures, 
        bool disableLensFlare, 
        bool disableBloom, 
        TAASettings taaSettings)
    {
        EnablePoolSizeToVramLimit = enablePoolSizeToVramLimit;
        DisableAntiAliasing = disableAntiAliasing;
        UseExperimentalStutterFix = useExperimentalStutterFix;
        ViewDistance = viewDistance;
        DisableFog = disableFog;
        ToneMapperSharpening = toneMapperSharpening;
        PotatoTextures = potatoTextures;
        DisableLensFlare = disableLensFlare;
        DisableBloom = disableBloom;
        TaaSettings = taaSettings;
    }

    public List<string> GetSettingsInfo()
    {
        _properties = this.GetType().GetProperties();
        _propertiesOfTAASettings = TaaSettings.GetType().GetProperties();

        List<string> returnList = new List<string>();
        
        foreach (var prop in _properties)
        {
            if(prop.Name == "TaaSettings") continue;
            returnList.Add(prop.Name + " = " + prop.GetValue(this));
        }

        foreach (var prop in _propertiesOfTAASettings)
        {
            returnList.Add(prop.Name + " = " + prop.GetValue(TaaSettings));
        }
        
        return returnList;
    }
    
}