namespace Monitor.Shared.Models.Studies;

/// <summary>
/// A RedCap class to describe a role on a Study.
/// </summary>
public class StudyRole
{
    public bool enabled { get; set; } = true;
    public int displaySequence { get; set; }
    public List<StudyRoleComponentPermissions> permissions { get; set; } = [];
    public bool useClassicMenu { get; set; } = false;
    public string name { get; set; } = string.Empty;
    public int applicationId { get; set; }
    public string menuButtonsIdentifier { get; set; } = string.Empty;
    public string scope { get; set; } = string.Empty;
    public int objectId { get; set; }
    public int menuId { get; set; }
    public int id { get; set; }
    public int parentRoleId { get; set; }
    public string applicationName { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
}
