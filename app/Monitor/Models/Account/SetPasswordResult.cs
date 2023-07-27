using Monitor.Models.User;

namespace Monitor.Models.Account;
public record SetPasswordResult
{
  public UserProfileModel? User { get; init; } = null;
  public bool? IsUnconfirmedAccount { get; init; } = null;
  public List<string>? Errors { get; init; } = new();
}
