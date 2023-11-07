namespace Monitor.Models.Studies;

public class StudyPartialModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StudyUser> Users { get; set; } = new List<StudyUser>();
}