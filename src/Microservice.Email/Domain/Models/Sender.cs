namespace Microservice.Email.Domain.Models;

/// <summary>
/// Represents an email sender with name and email address.
/// </summary>
public sealed class Sender
{
    /// <summary>
    /// Gets the display name of the sender. This is optional.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the email address of the sender.
    /// </summary>
    public required string Email { get; init; }
}
