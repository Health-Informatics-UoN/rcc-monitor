namespace Monitor.Shared.Models.Studies;

/// <summary>
/// A RedCap class to describe a Study Assignment.
/// </summary>
public class StudyAssignment
{
    public bool enabled { get; set; }
    public int studyId { get; set; }
    public int userId { get; set; }
    public string email { get; set; } = string.Empty;
    public int roleId { get; set; }
}