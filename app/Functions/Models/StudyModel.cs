namespace Functions.Models;

public class StudyModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public double StudyCapacityThreshold { get; set; }
    public TimeSpan StudyCapacityJobFrequency { get; set; }
    public DateTimeOffset StudyCapacityLastChecked { get; set; }
}
