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

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConsumerService"/> class.
    /// </summary>
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
        this.logger.LogInformation("RabbitMQ Consumer Service is starting...");

        try
        {
            await this.InitializeConnectionAsync(stoppingToken);
            await this.StartConsumersAsync(stoppingToken);

            this.logger.LogInformation("RabbitMQ Consumer Service started successfully");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogInformation("RabbitMQ Consumer Service is stopping...");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error in RabbitMQ Consumer Service");
            throw;
        }
    }

    private async Task InitializeConnectionAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = this.settings.HostName,
            Port = this.settings.Port,
            UserName = this.settings.Username,
            Password = this.settings.Password,
            VirtualHost = this.settings.VirtualHost
        };

        this.connection = await factory.CreateConnectionAsync(cancellationToken);

        this.emailChannel = await this.connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await this.emailChannel.QueueDeclareAsync(
            queue: this.settings.EmailQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        this.templatedEmailChannel = await this.connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await this.templatedEmailChannel.QueueDeclareAsync(
            queue: this.settings.TemplatedEmailQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        this.logger.LogInformation(
            "Connected to RabbitMQ at {HostName}:{Port}",
            this.settings.HostName,
            this.settings.Port);
    }

    private async Task StartConsumersAsync(CancellationToken cancellationToken)
    {
        // Consumer for plain emails
        var emailConsumer = new AsyncEventingBasicConsumer(this.emailChannel!);
        emailConsumer.ReceivedAsync += async (_, ea) =>
        {
            await this.HandleEmailMessageAsync(ea, cancellationToken);
        };

        await this.emailChannel!.BasicConsumeAsync(
            queue: this.settings.EmailQueueName,
            autoAck: false,
            consumer: emailConsumer,
            cancellationToken: cancellationToken);

        // Consumer for templated emails
        var templatedEmailConsumer = new AsyncEventingBasicConsumer(this.templatedEmailChannel!);
        templatedEmailConsumer.ReceivedAsync += async (_, ea) =>
        {
            await this.HandleTemplatedEmailMessageAsync(ea, cancellationToken);
        };

        await this.templatedEmailChannel!.BasicConsumeAsync(
            queue: this.settings.TemplatedEmailQueueName,
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
                this.logger.LogWarning("Received null or invalid email message");
                await this.emailChannel!.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                return;
            }

            using var scope = this.serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<AttachmentsWrapper<SendEmailRequest>>>();

            await handler.HandleAsync(message, cancellationToken);
            await this.emailChannel!.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error processing email message");
            await this.emailChannel!.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken);
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
                this.logger.LogWarning("Received null or invalid templated email message");
                await this.templatedEmailChannel!.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                return;
            }

            using var scope = this.serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<AttachmentsWrapper<SendTemplatedEmailRequest>>>();

            await handler.HandleAsync(message, cancellationToken);
            await this.templatedEmailChannel!.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error processing templated email message");
            await this.templatedEmailChannel!.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("RabbitMQ Consumer Service is stopping...");

        if (this.emailChannel is not null)
        {
            await this.emailChannel.CloseAsync(cancellationToken);
        }

        if (this.templatedEmailChannel is not null)
        {
            await this.templatedEmailChannel.CloseAsync(cancellationToken);
        }

        if (this.connection is not null)
        {
            await this.connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.emailChannel?.Dispose();
        this.templatedEmailChannel?.Dispose();
        this.connection?.Dispose();
        base.Dispose();
    }
}
