namespace Monitor.Shared.Models.Studies;

public class StudyModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public List<StudyUser>? Users { get; set; } = new List<StudyUser>();
    public string Instance { get; set; } = string.Empty;
    
}
