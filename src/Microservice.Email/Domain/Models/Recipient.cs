namespace Microservice.Email.Domain.Models;

/// <summary>
/// Represents an email recipient with identifier and email address.
/// </summary>
public sealed class Recipient
{
    /// <summary>
    /// Gets the unique identifier for the recipient.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the email address of the recipient.
    /// </summary>
    public required string Email { get; init; }
}
