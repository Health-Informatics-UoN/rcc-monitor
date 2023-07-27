namespace Monitor.Models;

public record RegistrationRuleModel
{
  public int Id { get; set; }
  public string Value { get; set; }
  public bool IsBlocked { get; set; }
  public DateTimeOffset Modified { get; set; }
}
