using Microservice.Email.Domain.Enums;

namespace Microservice.Email.Domain.Models;

/// <summary>
/// Represents the response model for an email message.
/// </summary>
public sealed class EmailResponse
{
    /// <summary>
    /// Gets the unique identifier for the email.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the sender of the email.
    /// </summary>
    public required Sender Sender { get; init; }

    /// <summary>
    /// Gets the collection of recipients for the email.
    /// </summary>
    public required IReadOnlyList<Recipient> Recipients { get; init; }

    /// <summary>
    /// Gets the date and time when the email was sent.
    /// </summary>
    public required DateTimeOffset SentDate { get; init; }

    /// <summary>
    /// Gets the current status of the email.
    /// </summary>
    public required EmailStatus EmailStatus { get; init; }
}
