using Monitor.Models.User;

namespace Monitor.Models.Account;
public record SetEmailResult
{
  public UserProfileModel? User { get; init; } = null;
  public bool? IsUnconfirmedAccount { get; init; } = null;
  public bool? IsExistingEmail { get; init; } = null;
  public bool? IsNotAllowlisted { get; init; } = null;
  public List<string>? Errors { get; init; } = new();
}
