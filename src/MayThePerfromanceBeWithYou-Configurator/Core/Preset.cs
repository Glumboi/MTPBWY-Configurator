namespace MayThePerfromanceBeWithYou_Configurator.Core;

public struct Preset
{
    public string IniUrl
    {
        get;
        private set;
    }   
    
    public string Name
    {
        get;
        private set;
    }
    
    public Preset(string iniUrl, string name)
    {
        IniUrl = iniUrl;
        Name = name;
    }
}