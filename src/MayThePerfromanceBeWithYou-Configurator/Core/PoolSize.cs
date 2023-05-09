namespace MayThePerfromanceBeWithYou_Configurator.Core;

public struct PoolSize
{
    public string VramSize
    {
        get;
        set;
    }   
    
    public int PoolSizeMatchingVram
    {
        get;
        set;
    }

    public PoolSize(string vram, int poolSize)
    {
        VramSize = vram;
        PoolSizeMatchingVram = poolSize;
    }
}