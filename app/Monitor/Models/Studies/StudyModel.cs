namespace Monitor.Models.Studies;

public class StudyModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public List<StudyUser>? Users { get; set; } = new List<StudyUser>();

    public override bool Equals(object? obj)
    {
        if (obj is not StudyModel other)
            return false;

        return Id == other.Id && Name == other.Name && ApiKey == other.ApiKey;
    }

    protected bool Equals(StudyModel other)
    {
        return Id == other.Id && Name == other.Name && ApiKey == other.ApiKey;
    }
    
}