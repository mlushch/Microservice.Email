using Microsoft.Extensions.Logging;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;

using Serilog.Context;

namespace Microservice.Email.Infrastructure.Messaging;

/// <summary>
/// Message handler for processing templated email send requests.
/// </summary>
public sealed class SendTemplatedEmailMessageHandler : IMessageHandler<AttachmentsWrapper<SendTemplatedEmailRequest>>
{
    private readonly IEmailService emailService;
    private readonly ILogger<SendTemplatedEmailMessageHandler> logger;

    public SendTemplatedEmailMessageHandler(
        IEmailService emailService,
        ILogger<SendTemplatedEmailMessageHandler> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task HandleAsync(BusMessage<AttachmentsWrapper<SendTemplatedEmailRequest>> message, CancellationToken cancellationToken = default)
    {
        // Push correlation ID from message to log context
        using (LogContext.PushProperty("CorrelationId", message.CorrelationId))
        {
            logger.LogInformation(
                "Processing templated email message with CorrelationId: {CorrelationId}, TemplateName: {TemplateName}",
                message.CorrelationId,
                message.Payload.Email.TemplateName);

            try
            {
                var response = await emailService.SendTemplatedAsync(message.Payload, cancellationToken);

                logger.LogInformation(
                    "Successfully processed templated email message with CorrelationId: {CorrelationId}, EmailId: {EmailId}",
                    message.CorrelationId,
                    response.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process templated email message with CorrelationId: {CorrelationId}",
                    message.CorrelationId);
                throw;
            }
        }
    }
}
