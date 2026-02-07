using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Models;

namespace Microservice.Email.Core.Interfaces;

/// <summary>
/// Service interface for email operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a plain email with optional attachments.
    /// </summary>
    /// <param name="request">The email request with attachments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email response with sending details.</returns>
    Task<EmailResponse> SendAsync(AttachmentsWrapper<SendEmailRequest> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a templated email with optional attachments.
    /// </summary>
    /// <param name="request">The templated email request with attachments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email response with sending details.</returns>
    Task<EmailResponse> SendTemplatedAsync(AttachmentsWrapper<SendTemplatedEmailRequest> request, CancellationToken cancellationToken = default);
}
