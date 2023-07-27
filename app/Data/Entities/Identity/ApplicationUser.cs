using Microsoft.AspNetCore.Identity;

namespace Monitor.Data.Entities.Identity;

public class ApplicationUser : IdentityUser
{
  [PersonalData]
  public string FullName { get; set; } = string.Empty;

  [PersonalData]
  public string UICulture { get; set; } = string.Empty;
  public int? TeamId { get; set; }
  public Team Team { get; set; } = null;
}
