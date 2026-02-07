using Microservice.Email.Domain.Models;

namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Represents a request to send an email using a template.
/// </summary>
public sealed class SendTemplatedEmailRequest
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
    /// Gets the name of the email template to use.
    /// </summary>
    public required string TemplateName { get; init; }

    /// <summary>
    /// Gets the dictionary of properties to substitute in the email template.
    /// </summary>
    public required Dictionary<string, object> TemplateProperties { get; init; }
}
