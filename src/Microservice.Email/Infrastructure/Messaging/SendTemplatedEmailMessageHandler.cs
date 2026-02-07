using Microsoft.Extensions.Logging;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;

namespace Microservice.Email.Infrastructure.Messaging;

/// <summary>
/// Message handler for processing templated email send requests.
/// </summary>
public sealed class SendTemplatedEmailMessageHandler : IMessageHandler<AttachmentsWrapper<SendTemplatedEmailRequest>>
{
    private readonly IEmailService emailService;
    private readonly ILogger<SendTemplatedEmailMessageHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendTemplatedEmailMessageHandler"/> class.
    /// </summary>
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
        this.logger.LogInformation(
            "Processing templated email message with CorrelationId: {CorrelationId}, TemplateName: {TemplateName}",
            message.CorrelationId,
            message.Payload.Email.TemplateName);

        try
        {
            var response = await this.emailService.SendTemplatedAsync(message.Payload, cancellationToken);

            this.logger.LogInformation(
                "Successfully processed templated email message with CorrelationId: {CorrelationId}, EmailId: {EmailId}",
                message.CorrelationId,
                response.Id);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Failed to process templated email message with CorrelationId: {CorrelationId}",
                message.CorrelationId);
            throw;
        }
    }
}
