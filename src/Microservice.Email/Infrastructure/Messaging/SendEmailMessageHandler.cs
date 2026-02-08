using Microsoft.Extensions.Logging;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;

using Serilog.Context;

namespace Microservice.Email.Infrastructure.Messaging;

/// <summary>
/// Message handler for processing plain email send requests.
/// </summary>
public sealed class SendEmailMessageHandler : IMessageHandler<AttachmentsWrapper<SendEmailRequest>>
{
    private readonly IEmailService emailService;
    private readonly ILogger<SendEmailMessageHandler> logger;

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
        // Push correlation ID from message to log context
        using (LogContext.PushProperty("CorrelationId", message.CorrelationId))
        {
            logger.LogInformation(
                "Processing email message with CorrelationId: {CorrelationId}",
                message.CorrelationId);

            try
            {
                var response = await emailService.SendAsync(message.Payload, cancellationToken);

                logger.LogInformation(
                    "Successfully processed email message with CorrelationId: {CorrelationId}, EmailId: {EmailId}",
                    message.CorrelationId,
                    response.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process email message with CorrelationId: {CorrelationId}",
                    message.CorrelationId);
                throw;
            }
        }
    }
}
