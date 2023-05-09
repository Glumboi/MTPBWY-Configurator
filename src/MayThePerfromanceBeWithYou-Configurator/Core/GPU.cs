using System;
using NvAPIWrapper.GPU;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class GPU
{
    public static int GetVramInGb()
    {
        double vramInGb = 0;
        try
        {
            PhysicalGPU[] gpus = PhysicalGPU.GetPhysicalGPUs();
            vramInGb = gpus[0].MemoryInformation.DedicatedVideoMemoryInkB / 1048576.0;
        }
        catch
        {
            return (int)vramInGb;
        }
        
        return (int)vramInGb;
    }
}