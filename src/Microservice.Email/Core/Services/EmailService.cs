using System.Diagnostics;

using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Metrics;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Entities;
using Microservice.Email.Domain.Enums;
using Microservice.Email.Domain.Models;
using Microservice.Email.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Microservice.Email.Core.Services;

/// <summary>
/// Service for email operations.
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly EmailDbContext dbContext;
    private readonly ISmtpService smtpService;
    private readonly ITemplateService templateService;
    private readonly IValidator<SendEmailRequest> emailValidator;
    private readonly IValidator<SendTemplatedEmailRequest> templatedEmailValidator;
    private readonly ILogger<EmailService> logger;

    public EmailService(
        EmailDbContext dbContext,
        ISmtpService smtpService,
        ITemplateService templateService,
        IValidator<SendEmailRequest> emailValidator,
        IValidator<SendTemplatedEmailRequest> templatedEmailValidator,
        ILogger<EmailService> logger)
    {
        this.dbContext = dbContext;
        this.smtpService = smtpService;
        this.templateService = templateService;
        this.emailValidator = emailValidator;
        this.templatedEmailValidator = templatedEmailValidator;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<EmailResponse> SendAsync(AttachmentsWrapper<SendEmailRequest> request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        const string templateName = "plain";

        var validationResult = emailValidator.Validate(request.Email);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var emailEntity = new EmailEntity
        {
            Subject = request.Email.Subject,
            Body = request.Email.Body,
            SenderName = request.Email.Sender.Name ?? request.Email.Sender.Email,
            SenderEmail = request.Email.Sender.Email,
            SentDate = DateTimeOffset.UtcNow,
            EmailStatus = EmailStatus.Pending,
            Recipients = request.Email.Recipients.Select(r => new RecipientEntity
            {
                Email = r,
                EmailEntity = null!
            }).ToList()
        };

        dbContext.Emails.Add(emailEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            emailEntity.EmailStatus = EmailStatus.Sending;
            await dbContext.SaveChangesAsync(cancellationToken);

            await smtpService.SendAsync(
                request.Email.Sender,
                request.Email.Recipients,
                request.Email.Subject,
                request.Email.Body,
                isHtml: false,
                request.Attachments,
                cancellationToken);

            emailEntity.EmailStatus = EmailStatus.Sent;
            emailEntity.SentDate = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            stopwatch.Stop();
            EmailMetrics.RecordEmailSent(templateName, stopwatch.Elapsed);

            logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", request.Email.Recipients));
        }
        catch (Exception ex)
        {
            EmailMetrics.RecordEmailFailed(templateName);

            logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", request.Email.Recipients));
            emailEntity.EmailStatus = EmailStatus.Failed;
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new EmailSendException("Failed to send email.", ex);
        }

        return MapToResponse(emailEntity);
    }

    /// <inheritdoc />
    public async Task<EmailResponse> SendTemplatedAsync(AttachmentsWrapper<SendTemplatedEmailRequest> request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var validationResult = templatedEmailValidator.Validate(request.Email);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var renderedBody = await templateService.RenderAsync(
            request.Email.TemplateName,
            request.Email.TemplateProperties,
            cancellationToken);

        var emailEntity = new EmailEntity
        {
            Subject = request.Email.TemplateProperties.TryGetValue("Subject", out var subject)
                ? subject?.ToString() ?? request.Email.TemplateName
                : request.Email.TemplateName,
            Body = renderedBody,
            SenderName = request.Email.Sender.Name ?? request.Email.Sender.Email,
            SenderEmail = request.Email.Sender.Email,
            SentDate = DateTimeOffset.UtcNow,
            EmailStatus = EmailStatus.Pending,
            Recipients = request.Email.Recipients.Select(r => new RecipientEntity
            {
                Email = r,
                EmailEntity = null!
            }).ToList()
        };

        dbContext.Emails.Add(emailEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            emailEntity.EmailStatus = EmailStatus.Sending;
            await dbContext.SaveChangesAsync(cancellationToken);

            await smtpService.SendAsync(
                request.Email.Sender,
                request.Email.Recipients,
                emailEntity.Subject,
                renderedBody,
                isHtml: true,
                request.Attachments,
                cancellationToken);

            emailEntity.EmailStatus = EmailStatus.Sent;
            emailEntity.SentDate = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            stopwatch.Stop();
            EmailMetrics.RecordEmailSent(request.Email.TemplateName, stopwatch.Elapsed);

            logger.LogInformation("Templated email sent successfully to {Recipients} using template {TemplateName}",
                string.Join(", ", request.Email.Recipients), request.Email.TemplateName);
        }
        catch (Exception ex)
        {
            EmailMetrics.RecordEmailFailed(request.Email.TemplateName);

            logger.LogError(ex, "Failed to send templated email to {Recipients} using template {TemplateName}",
                string.Join(", ", request.Email.Recipients), request.Email.TemplateName);
            emailEntity.EmailStatus = EmailStatus.Failed;
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new EmailSendException("Failed to send templated email.", ex);
        }

        return MapToResponse(emailEntity);
    }

    private static EmailResponse MapToResponse(EmailEntity entity)
    {
        return new EmailResponse
        {
            Id = entity.Id,
            Sender = new Sender
            {
                Name = entity.SenderName,
                Email = entity.SenderEmail
            },
            Recipients = entity.Recipients.Select(r => new Recipient
            {
                Id = r.Id,
                Email = r.Email
            }).ToList(),
            SentDate = entity.SentDate,
            EmailStatus = entity.EmailStatus
        };
    }
}
