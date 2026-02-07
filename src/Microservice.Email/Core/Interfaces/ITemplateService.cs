using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Models;

namespace Microservice.Email.Core.Interfaces;

/// <summary>
/// Service interface for email template operations.
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Gets all email templates.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of email template responses.</returns>
    Task<IReadOnlyList<EmailTemplateResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="request">The template creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CreateAsync(CreateEmailTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="templateId">The ID of the template to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAsync(int templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders a template with the provided properties.
    /// </summary>
    /// <param name="templateName">The name of the template to render.</param>
    /// <param name="properties">The properties to substitute in the template.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rendered HTML string.</returns>
    Task<string> RenderAsync(string templateName, Dictionary<string, object> properties, CancellationToken cancellationToken = default);
}
