using System;

namespace MayThePerfromanceBeWithYou_Configurator.Core;

public class MathHelpers
{
    public static bool IsFloatOrInt(string value)
    {
        int intValue;
        float floatValue;
        return Int32.TryParse(value, out intValue);
    }
    
    public static float ParseFloat(string src)
    {
        float rtrn;
        if (float.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        return 0f;
    }

    public static int ParseInt(string src, bool useNullAsReturn = false)
    {
        int rtrn;
        if (Int32.TryParse(src, out rtrn))
        {
            return rtrn;
        }

        if (useNullAsReturn) return 0;
        return 9999;
    }
}