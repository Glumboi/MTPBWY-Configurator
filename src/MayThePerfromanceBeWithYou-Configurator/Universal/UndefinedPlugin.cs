namespace MayThePerfromanceBeWithYou_Configurator.Universal;

public class UndefinedPlugin : StandardPluginImplementations
{
    public override string PluginDebugIdentifier { get; set; }

    public UndefinedPlugin(string identifier) => PluginDebugIdentifier = identifier;
}