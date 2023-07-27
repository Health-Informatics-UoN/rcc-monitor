namespace Monitor.Data.Entities;

public class RegistrationRule
{
  public int Id { get; set; }
  public string Value { get; set; }
  public DateTimeOffset Modified { get; set; } = DateTimeOffset.UtcNow;
  public bool IsBlocked { get; set; }
}
