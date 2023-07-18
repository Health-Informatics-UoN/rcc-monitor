#nullable enable
namespace Francois.FunctionApp.Models.Emails;

public record EmailAddress(string Address)
{
    public string? Name { get; init; }
}