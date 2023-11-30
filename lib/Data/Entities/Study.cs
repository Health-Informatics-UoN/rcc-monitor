using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitor.Data.Entities;

public class Study
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int RedCapId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public ICollection<StudyUser> Users { get; set; } = null!;
    public Instance Instance { get; set; } = new();
    public bool StudyCapacityAlertsActivated { get; set; } = false;
    public double StudyCapacityThreshold { get; set; }
    public TimeSpan StudyCapacityJobFrequency { get; set; }
    public DateTimeOffset StudyCapacityLastChecked { get; set; }
    public bool StudyCapacityAlert { get; set; } = false;
    public ICollection<StudyGroup> StudyGroups { get; set; } = new List<StudyGroup>();

}
