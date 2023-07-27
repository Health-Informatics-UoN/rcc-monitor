using System.ComponentModel.DataAnnotations;

namespace Monitor.Data.Entities;

public class RegistrationAllowlistEntry
{
  [Key] // TODO: partial email allow listing e.g. nottingham.ac.uk
  public string EmailAddress { get; set; } = string.Empty;
}
