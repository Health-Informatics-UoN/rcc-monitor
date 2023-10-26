namespace Monitor.Data.Entities;

public class StudyUser
{
    public int StudyId { get; set; }
    public Study Study { get; set; } = new Study();
    public string UserId { get; set; } = string.Empty;
}
