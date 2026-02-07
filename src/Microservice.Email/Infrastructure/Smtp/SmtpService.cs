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

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpService"/> class.
    /// </summary>
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
        var senderEmail = sender.Email ?? this.settings.DefaultSenderEmail
            ?? throw new EmailSendException("Sender email is required.");
        var senderName = sender.Name ?? this.settings.DefaultSenderName ?? senderEmail;

        int attempts = 0;
        Exception? lastException = null;

        while (attempts < this.settings.MaxRetryAttempts)
        {
            attempts++;
            try
            {
                var email = this.fluentEmail
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
                    this.logger.LogInformation(
                        "Email sent successfully to {Recipients} on attempt {Attempt}",
                        string.Join(", ", recipients),
                        attempts);
                    return;
                }

                lastException = new EmailSendException(
                    $"Failed to send email: {string.Join(", ", response.ErrorMessages)}");

                this.logger.LogWarning(
                    "Email sending failed on attempt {Attempt}: {Errors}",
                    attempts,
                    string.Join(", ", response.ErrorMessages));
            }
            catch (Exception ex)
            {
                lastException = ex;
                this.logger.LogWarning(
                    ex,
                    "Email sending failed on attempt {Attempt}",
                    attempts);
            }

            if (attempts < this.settings.MaxRetryAttempts)
            {
                await Task.Delay(this.settings.RetryDelayMilliseconds, cancellationToken);
            }
        }

        this.logger.LogError(
            lastException,
            "Email sending failed after {MaxAttempts} attempts to {Recipients}",
            this.settings.MaxRetryAttempts,
            string.Join(", ", recipients));

        throw new EmailSendException(
            $"Failed to send email after {this.settings.MaxRetryAttempts} attempts.",
            lastException ?? new Exception("Unknown error"));
    }
}
