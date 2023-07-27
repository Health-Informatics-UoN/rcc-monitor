using System.ComponentModel.DataAnnotations;

namespace Monitor.Models.Account;

public record UserTokenModel(
    [Required]
    string UserId,
    [Required]
    string Token);

