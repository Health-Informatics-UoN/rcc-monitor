using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitor.Data.Entities;

public class Config
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ConfigType Type { get; set; }
}

public enum ConfigType
{
    TimeSpan,
    Double,
    Int
}