using Monitor.Data.Entities;

namespace Monitor.Models.Config;

public class ConfigModel
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ConfigType Type { get; set; }
}