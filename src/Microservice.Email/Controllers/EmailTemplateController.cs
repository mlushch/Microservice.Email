using Microsoft.AspNetCore.Mvc;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Models;

namespace Microservice.Email.Controllers;

/// <summary>
/// Controller for email template management operations.
/// </summary>
[ApiController]
[Route("api/email-templates")]
[Produces("application/json")]
public sealed class EmailTemplateController : ControllerBase
{
    private readonly ITemplateService templateService;
    private readonly ILogger<EmailTemplateController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTemplateController"/> class.
    /// </summary>
    public EmailTemplateController(ITemplateService templateService, ILogger<EmailTemplateController> logger)
    {
        this.templateService = templateService;
        this.logger = logger;
    }

    /// <summary>
    /// Gets all email templates.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of email templates.</returns>
    /// <response code="200">Returns the list of templates.</response>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IReadOnlyList<EmailTemplateResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EmailTemplateResponse>>> GetAll(CancellationToken cancellationToken)
    {
        this.logger.LogDebug("Retrieving all email templates");

        var templates = await this.templateService.GetAllAsync(cancellationToken);

        this.logger.LogInformation("Retrieved {TemplateCount} email templates", templates.Count);

        return this.Ok(templates);
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="request">The template creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Template created successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="409">Template with the same name already exists.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromForm] CreateEmailTemplateRequest request,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Creating email template with name {TemplateName}", request.Name);

        await this.templateService.CreateAsync(request, cancellationToken);

        this.logger.LogInformation("Email template {TemplateName} created successfully", request.Name);

        return this.NoContent();
    }

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="templateId">The ID of the template to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Template deleted successfully.</response>
    /// <response code="404">Template not found.</response>
    [HttpDelete("{templateId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int templateId, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Deleting email template with ID {TemplateId}", templateId);

        await this.templateService.DeleteAsync(templateId, cancellationToken);

        this.logger.LogInformation("Email template with ID {TemplateId} deleted successfully", templateId);

        return this.NoContent();
    }
}
