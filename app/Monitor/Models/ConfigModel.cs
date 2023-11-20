using Monitor.Data.Entities;

namespace Monitor.Models;

public class ConfigModel
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ConfigType Type { get; set; }
}