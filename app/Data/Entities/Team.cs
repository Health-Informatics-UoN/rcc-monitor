using Monitor.Data.Entities.Identity;

namespace Monitor.Data.Entities;

public class Team
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ApplicationUser> Members { get; set; } = new();
  public bool InternalUser { get; set; } = false;
}
