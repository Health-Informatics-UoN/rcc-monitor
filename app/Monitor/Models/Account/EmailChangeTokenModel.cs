using System.ComponentModel.DataAnnotations;

namespace Monitor.Models.Account;

public record EmailChangeTokenModel(
  [Required]
  string UserId,
  [Required]
  string Token,
  [Required]
  string NewEmail);
