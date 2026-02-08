using System.Text;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Domain.Contracts;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microservice.Email.Infrastructure.Messaging;

/// <summary>
/// Background service that consumes messages from RabbitMQ queues.
/// </summary>
public sealed class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly RabbitMqSettings settings;
    private readonly ILogger<RabbitMqConsumerService> logger;
    private IConnection? connection;
    private IChannel? emailChannel;
    private IChannel? templatedEmailChannel;

    public RabbitMqConsumerService(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqSettings> settings,
        ILogger<RabbitMqConsumerService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.settings = settings.Value;
        this.logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RabbitMQ Consumer Service is starting...");

        try
        {
            await InitializeConnectionAsync(stoppingToken);
            await StartConsumersAsync(stoppingToken);

            logger.LogInformation("RabbitMQ Consumer Service started successfully");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("RabbitMQ Consumer Service is stopping...");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RabbitMQ Consumer Service");
            throw;
        }
    }

    private async Task InitializeConnectionAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };

        if (settings.UseSsl)
        {
            factory.Ssl.Enabled = true;
        }
        connection = await factory.CreateConnectionAsync(cancellationToken);

        emailChannel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await emailChannel.QueueDeclareAsync(
            queue: settings.EmailQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        templatedEmailChannel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await templatedEmailChannel.QueueDeclareAsync(
            queue: settings.TemplatedEmailQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Connected to RabbitMQ at {HostName}:{Port}",
            settings.HostName,
            settings.Port);
    }

    private async Task StartConsumersAsync(CancellationToken cancellationToken)
    {
        // Consumer for plain emails
        var emailConsumer = new AsyncEventingBasicConsumer(emailChannel!);
        emailConsumer.ReceivedAsync += async (_, ea) =>
        {
            await HandleEmailMessageAsync(ea, cancellationToken);
        };

        await emailChannel!.BasicConsumeAsync(
            queue: settings.EmailQueueName,
            autoAck: false,
            consumer: emailConsumer,
            cancellationToken: cancellationToken);

        // Consumer for templated emails
        var templatedEmailConsumer = new AsyncEventingBasicConsumer(templatedEmailChannel!);
        templatedEmailConsumer.ReceivedAsync += async (_, ea) =>
        {
            await HandleTemplatedEmailMessageAsync(ea, cancellationToken);
        };

        await templatedEmailChannel!.BasicConsumeAsync(
            queue: settings.TemplatedEmailQueueName,
            autoAck: false,
            consumer: templatedEmailConsumer,
            cancellationToken: cancellationToken);
    }

    private async Task HandleEmailMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        try
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<BusMessage<AttachmentsWrapper<SendEmailRequest>>>(body);

            if (message is null)
            {
                logger.LogWarning("Received null or invalid email message");
                await emailChannel!.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<AttachmentsWrapper<SendEmailRequest>>>();

            await handler.HandleAsync(message, cancellationToken);
            await emailChannel!.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing email message");
            await emailChannel!.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken);
        }
    }

    private async Task HandleTemplatedEmailMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        try
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<BusMessage<AttachmentsWrapper<SendTemplatedEmailRequest>>>(body);

            if (message is null)
            {
                logger.LogWarning("Received null or invalid templated email message");
                await templatedEmailChannel!.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<AttachmentsWrapper<SendTemplatedEmailRequest>>>();

            await handler.HandleAsync(message, cancellationToken);
            await templatedEmailChannel!.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing templated email message");
            await templatedEmailChannel!.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("RabbitMQ Consumer Service is stopping...");

        if (emailChannel is not null)
        {
            await emailChannel.CloseAsync(cancellationToken);
        }

        if (templatedEmailChannel is not null)
        {
            await templatedEmailChannel.CloseAsync(cancellationToken);
        }

        if (connection is not null)
        {
            await connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        emailChannel?.Dispose();
        templatedEmailChannel?.Dispose();
        connection?.Dispose();
        base.Dispose();
    }
}
