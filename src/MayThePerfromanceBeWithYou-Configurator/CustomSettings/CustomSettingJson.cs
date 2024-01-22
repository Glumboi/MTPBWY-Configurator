using System;
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

    [JsonPropertyName("ignoreIfDefault")] public bool IgnoreIfDefault { get; set; }

    public string InitialDefault { get; set; }

    public CustomSettingJson(string settingName, string settingSection, string settingKey, string defaultValue,
        string maxValue, string minValue, string dependsOn, string initialDefault)
    {
        SettingName = settingName;
        SettingSection = settingSection;
        SettingKey = settingKey;
        DefaultValue = defaultValue;
        MaxValue = maxValue;
        MinValue = minValue;
        DependsOn = dependsOn;
        InitialDefault = defaultValue;
    }
}