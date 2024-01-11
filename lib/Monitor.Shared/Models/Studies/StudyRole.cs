namespace Monitor.Shared.Models.Studies;

/// <summary>
/// A RedCap class to describe a role on a Study.
/// </summary>
public class StudyRole
{
    public string name { get; set; } = string.Empty;
    public bool enabled { get; set; } = true;
    public List<StudyRoleComponentPermissions> permissions { get; set; } = [];
}
