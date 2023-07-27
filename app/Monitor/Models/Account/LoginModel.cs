using System.ComponentModel.DataAnnotations;

namespace Monitor.Models.Account;

public record LoginModel(
  [Required]
  [EmailAddress]
  string Username,

  [Required]
  [DataType(DataType.Password)]
  string Password
);
