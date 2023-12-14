using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitor.Data.Entities;

/// <summary>
/// Represents a study entity.
/// </summary>
public class Study
{
    /// <summary>
    /// The RedCap Id of a study.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int RedCapId { get; set; }
    /// <summary>
    /// The name of the study.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// API key required to gain access to the study.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
    /// <summary>
    /// Users associated with the study.
    /// </summary>
    public ICollection<StudyUser> Users { get; set; } = null!;
    /// <summary>
    /// The instance of the study i.e Build, Production, UAT
    /// </summary>
    public Instance Instance { get; set; } = new();
    /// <summary>
    /// This indicates whether study capacity alerts are activated or not.
    /// </summary>
    public bool StudyCapacityAlertsActivated { get; set; } = false;
    /// <summary>
    /// The threshold for when an alert should be triggered for the study capacity.
    /// </summary>
    public double StudyCapacityThreshold { get; set; }
    /// <summary>
    /// The threshold for any enrolled patients on a study.
    /// </summary>
    public int SubjectsEnrolledThreshold { get; set; }
    /// <summary>
    /// Frequency of the job to check if study is at capacity.
    /// </summary>
    public TimeSpan StudyCapacityJobFrequency { get; set; }
    /// <summary>
    /// When last was the study capacity checked (date and time).
    /// </summary>
    public DateTimeOffset StudyCapacityLastChecked { get; set; }
    /// <summary>
    /// If there is any study capacity alert.
    /// </summary>
    public bool StudyCapacityAlert { get; set; } = false;
    /// <summary>
    /// The number of subjects enrolled to a study.
    /// </summary>
    public int SubjectsEnrolled { get; set; }
    /// <summary>
    /// List of study groups.
    /// </summary>
    public ICollection<StudyGroup> StudyGroups { get; set; } = new List<StudyGroup>();

}
