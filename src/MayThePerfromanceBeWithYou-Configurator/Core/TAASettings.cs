namespace MayThePerfromanceBeWithYou_Configurator.Core;

public struct TAASettings
{
    public int TaaResolution    
    {
        get; 
        set;
    }
    
    public bool TaaUpscaling    
    {
        get; 
        set;
    }
    
    public bool TaaGen5   
    {
        get;
        set;
    }
    
    public TAASettings(bool taaUpscaling, int taaResolution, bool taaGen5)
    {
        TaaUpscaling = taaUpscaling;
        TaaResolution = taaResolution;
        TaaGen5 = taaGen5;
    }
}