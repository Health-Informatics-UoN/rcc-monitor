namespace Monitor.Models.Studies;

public class StudyPartialModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public bool StudyCapacityAlert { get; set; } = false;
    public bool ProductionSubjectsEnteredAlert { get; set; } = false;
    public List<StudyUser>? Users { get; set; } = new List<StudyUser>();
    public ICollection<StudyGroupModel> StudyGroup { get; set; } = new List<StudyGroupModel>();
    public int SubjectsEnrolled { get; set; }
    public bool StudyCapacityAlertsActivated { get; set; } = false;
    public double StudyCapacityThreshold { get; set; }
    public int SubjectsEnrolledThreshold { get; set; }
    public string StudyCapacityJobFrequency { get; set; } = string.Empty;
    public string StudyCapacityLastChecked { get; set; } = string.Empty;
    
}