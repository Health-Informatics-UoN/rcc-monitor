using System.ComponentModel.DataAnnotations;

namespace Monitor.Models;

public record CreateRegistrationRuleModel(
  [Required] string Value,

  [Required] bool IsBlocked
);
