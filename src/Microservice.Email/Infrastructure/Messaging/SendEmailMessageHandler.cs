using Microsoft.Extensions.Logging;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;

namespace Microservice.Email.Infrastructure.Messaging;

/// <summary>
/// Message handler for processing plain email send requests.
/// </summary>
public sealed class SendEmailMessageHandler : IMessageHandler<AttachmentsWrapper<SendEmailRequest>>
{
    private readonly IEmailService emailService;
    private readonly ILogger<SendEmailMessageHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendEmailMessageHandler"/> class.
    /// </summary>
    public SendEmailMessageHandler(
        IEmailService emailService,
        ILogger<SendEmailMessageHandler> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task HandleAsync(BusMessage<AttachmentsWrapper<SendEmailRequest>> message, CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation(
            "Processing email message with CorrelationId: {CorrelationId}",
            message.CorrelationId);

        try
        {
            var response = await this.emailService.SendAsync(message.Payload, cancellationToken);

            this.logger.LogInformation(
                "Successfully processed email message with CorrelationId: {CorrelationId}, EmailId: {EmailId}",
                message.CorrelationId,
                response.Id);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Failed to process email message with CorrelationId: {CorrelationId}",
                message.CorrelationId);
            throw;
        }
    }
}
