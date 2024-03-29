namespace Monitor.Models.SyntheticData;

public class FieldMappings
{
    public List<FieldMapping>? Mappings { get; set; }
}


public class FieldMapping
{
    public string FieldName { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
    public string MinValue { get; set; } = string.Empty;
    public string MaxValue { get; set; } = string.Empty;
}