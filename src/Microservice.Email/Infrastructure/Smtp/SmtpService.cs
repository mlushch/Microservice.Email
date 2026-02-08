using FluentEmail.Core;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Models;

using DomainAttachment = Microservice.Email.Domain.Contracts.Attachment;
using FluentAttachment = FluentEmail.Core.Models.Attachment;

namespace Microservice.Email.Infrastructure.Smtp;

/// <summary>
/// SMTP implementation using FluentEmail for email delivery.
/// </summary>
public sealed class SmtpService : ISmtpService
{
    private readonly IFluentEmail fluentEmail;
    private readonly SmtpSettings settings;
    private readonly ILogger<SmtpService> logger;

    public SmtpService(
        IFluentEmail fluentEmail,
        IOptions<SmtpSettings> settings,
        ILogger<SmtpService> logger)
    {
        this.fluentEmail = fluentEmail;
        this.settings = settings.Value;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task SendAsync(
        Sender sender,
        string[] recipients,
        string subject,
        string body,
        bool isHtml = false,
        DomainAttachment[]? attachments = null,
        CancellationToken cancellationToken = default)
    {
        var senderEmail = sender.Email ?? settings.DefaultSenderEmail
            ?? throw new EmailSendException("Sender email is required.");
        var senderName = sender.Name ?? settings.DefaultSenderName ?? senderEmail;

        int attempts = 0;
        Exception? lastException = null;

        while (attempts < settings.MaxRetryAttempts)
        {
            attempts++;
            try
            {
                var email = fluentEmail
                    .SetFrom(senderEmail, senderName)
                    .Subject(subject);

                foreach (var recipient in recipients)
                {
                    email = email.To(recipient);
                }

                if (isHtml)
                {
                    email = email.Body(body, true);
                }
                else
                {
                    email = email.Body(body);
                }

                if (attachments is not null)
                {
                    foreach (var attachment in attachments)
                    {
                        var bytes = Convert.FromBase64String(attachment.ContentBase64);
                        var stream = new MemoryStream(bytes);
                        email = email.Attach(new FluentEmail.Core.Models.Attachment
                        {
                            Data = stream,
                            Filename = attachment.FileName,
                            ContentType = attachment.ContentType ?? "application/octet-stream"
                        });
                    }
                }

                var response = await email.SendAsync(cancellationToken);

                if (response.Successful)
                {
                    logger.LogInformation(
                        "Email sent successfully to {Recipients} on attempt {Attempt}",
                        string.Join(", ", recipients),
                        attempts);
                    return;
                }

                lastException = new EmailSendException(
                    $"Failed to send email: {string.Join(", ", response.ErrorMessages)}");

                logger.LogWarning(
                    "Email sending failed on attempt {Attempt}: {Errors}",
                    attempts,
                    string.Join(", ", response.ErrorMessages));
            }
            catch (Exception ex)
            {
                lastException = ex;
                logger.LogWarning(
                    ex,
                    "Email sending failed on attempt {Attempt}",
                    attempts);
            }

            if (attempts < settings.MaxRetryAttempts)
            {
                await Task.Delay(settings.RetryDelayMilliseconds, cancellationToken);
            }
        }

        logger.LogError(
            lastException,
            "Email sending failed after {MaxAttempts} attempts to {Recipients}",
            settings.MaxRetryAttempts,
            string.Join(", ", recipients));

        throw new EmailSendException(
            $"Failed to send email after {settings.MaxRetryAttempts} attempts.",
            lastException ?? new Exception("Unknown error"));
    }
}
