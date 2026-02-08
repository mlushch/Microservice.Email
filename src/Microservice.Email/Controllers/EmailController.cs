using Microsoft.AspNetCore.Mvc;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Models;

namespace Microservice.Email.Controllers;

/// <summary>
/// Controller for email sending operations.
/// </summary>
[ApiController]
[Route("api/email")]
[Produces("application/json")]
public sealed class EmailController : ControllerBase
{
    private readonly IEmailService emailService;
    private readonly ILogger<EmailController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailController"/> class.
    /// </summary>
    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    /// <summary>
    /// Sends a plain email with optional base64-encoded attachments.
    /// </summary>
    /// <param name="request">The email request with attachments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email response.</returns>
    /// <response code="200">Email sent successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(EmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmailResponse>> Send(
        [FromBody] AttachmentsWrapper<SendEmailRequest> request,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Sending plain email to {RecipientCount} recipients",
            request.Email.Recipients.Length);

        var response = await this.emailService.SendAsync(request, cancellationToken);

        this.logger.LogInformation(
            "Plain email sent successfully with ID {EmailId}",
            response.Id);

        return this.Ok(response);
    }

    /// <summary>
    /// Sends a templated email with optional base64-encoded attachments.
    /// </summary>
    /// <param name="request">The templated email request with attachments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email response.</returns>
    /// <response code="200">Email sent successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">Template not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("send/templated")]
    [ProducesResponseType(typeof(EmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmailResponse>> SendTemplated(
        [FromBody] AttachmentsWrapper<SendTemplatedEmailRequest> request,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Sending templated email using template {TemplateName} to {RecipientCount} recipients",
            request.Email.TemplateName,
            request.Email.Recipients.Length);

        var response = await this.emailService.SendTemplatedAsync(request, cancellationToken);

        this.logger.LogInformation(
            "Templated email sent successfully with ID {EmailId} using template {TemplateName}",
            response.Id,
            request.Email.TemplateName);

        return this.Ok(response);
    }

    /// <summary>
    /// Sends a plain email with form-data file attachments.
    /// </summary>
    /// <param name="request">The email request with files.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email response.</returns>
    /// <response code="200">Email sent successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("send/formFiles")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(EmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmailResponse>> SendWithFormFiles(
        [FromForm] FormFilesWrapper<SendEmailRequest> request,
        CancellationToken cancellationToken)
    {
        var attachments = await ConvertFormFilesToAttachmentsAsync(request.Files, cancellationToken);
        var wrapper = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = request.Email,
            Attachments = attachments
        };

        var response = await this.emailService.SendAsync(wrapper, cancellationToken);
        return this.Ok(response);
    }

    /// <summary>
    /// Sends a templated email with form-data file attachments.
    /// </summary>
    /// <param name="request">The templated email request with files.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email response.</returns>
    /// <response code="200">Email sent successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">Template not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("send/templated/formFiles")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(EmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmailResponse>> SendTemplatedWithFormFiles(
        [FromForm] FormFilesWrapper<SendTemplatedEmailRequest> request,
        CancellationToken cancellationToken)
    {
        var attachments = await ConvertFormFilesToAttachmentsAsync(request.Files, cancellationToken);
        var wrapper = new AttachmentsWrapper<SendTemplatedEmailRequest>
        {
            Email = request.Email,
            Attachments = attachments
        };

        var response = await this.emailService.SendTemplatedAsync(wrapper, cancellationToken);
        return this.Ok(response);
    }

    private static async Task<Attachment[]?> ConvertFormFilesToAttachmentsAsync(IFormFile[]? files, CancellationToken cancellationToken)
    {
        if (files is null || files.Length == 0)
        {
            return null;
        }

        var attachments = new Attachment[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            using var memoryStream = new MemoryStream();
            await files[i].CopyToAsync(memoryStream, cancellationToken);
            var base64Content = Convert.ToBase64String(memoryStream.ToArray());

            attachments[i] = new Attachment
            {
                FileName = files[i].FileName,
                ContentBase64 = base64Content,
                ContentType = files[i].ContentType
            };
        }

        return attachments;
    }
}
