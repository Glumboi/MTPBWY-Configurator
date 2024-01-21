using System.Text.Json.Serialization;

namespace MayThePerfromanceBeWithYou_Configurator.CustomSettings;

public class CustomSettingJson
{
    [JsonPropertyName("settingName")] public string SettingName { get; set; }

    [JsonPropertyName("settingType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CustomSettingType SettingType { get; set; }

    [JsonPropertyName("settingSection")] public string SettingSection { get; set; }

    [JsonPropertyName("settingKey")] public string SettingKey { get; set; }

    [JsonPropertyName("defaultValue")] public string DefaultValue { get; set; }

    [JsonPropertyName("maxValue")] public string MaxValue { get; set; }

    [JsonPropertyName("minValue")] public string MinValue { get; set; }

    [JsonPropertyName("dependsOn")] public string DependsOn { get; set; }
}