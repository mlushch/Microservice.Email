using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Models;

namespace Microservice.Email.Core.Interfaces;

/// <summary>
/// Service interface for SMTP email delivery operations.
/// </summary>
public interface ISmtpService
{
    /// <summary>
    /// Sends an email via SMTP.
    /// </summary>
    /// <param name="sender">The email sender information.</param>
    /// <param name="recipients">The list of recipient email addresses.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body content.</param>
    /// <param name="isHtml">Whether the body is HTML content.</param>
    /// <param name="attachments">Optional attachments to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SendAsync(
        Sender sender,
        string[] recipients,
        string subject,
        string body,
        bool isHtml = false,
        Attachment[]? attachments = null,
        CancellationToken cancellationToken = default);
}
