using Microservice.Email.Domain.Models;

namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Represents a request to send a plain email.
/// </summary>
public sealed class SendEmailRequest
{
    /// <summary>
    /// Gets the sender of the email.
    /// </summary>
    public required Sender Sender { get; init; }

    /// <summary>
    /// Gets the list of recipient email addresses.
    /// </summary>
    public required string[] Recipients { get; init; }

    /// <summary>
    /// Gets the subject line of the email.
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// Gets the body content of the email.
    /// </summary>
    public required string Body { get; init; }
}
