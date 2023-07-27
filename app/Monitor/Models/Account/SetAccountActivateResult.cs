using Monitor.Models.User;

namespace Monitor.Models.Account;

public record SetAccountActivateResult
{
  public UserProfileModel? User { get; init; } = null;
  public bool? IsActivationTokenInvalid { get; init; } = null;
  public List<string> Errors { get; init; } = new();
};


