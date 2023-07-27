
using System.ComponentModel.DataAnnotations;

namespace Monitor.Models.Account;

public record RegisterAccountModel(
  [Required]
  [DataType(DataType.Text)]
  string FullName,

  [Required]
  [EmailAddress]
  string Email,

  [Required]
  [DataType(DataType.Password)]
  string Password
);
