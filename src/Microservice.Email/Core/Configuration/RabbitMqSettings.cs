namespace Microservice.Email.Core.Configuration;

/// <summary>
/// Configuration settings for RabbitMQ messaging.
/// </summary>
public sealed class RabbitMqSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "RabbitMq";

    /// <summary>
    /// Gets or sets the RabbitMQ host name.
    /// </summary>
    public required string HostName { get; init; }

    /// <summary>
    /// Gets or sets the RabbitMQ port.
    /// </summary>
    public int Port { get; init; } = 5672;

    /// <summary>
    /// Gets or sets the RabbitMQ username.
    /// </summary>
    public string Username { get; init; } = "guest";

    /// <summary>
    /// Gets or sets the RabbitMQ password.
    /// </summary>
    public string Password { get; init; } = "guest";

    /// <summary>
    /// Gets or sets the RabbitMQ virtual host.
    /// </summary>
    public string VirtualHost { get; init; } = "/";

    /// <summary>
    /// Gets or sets the queue name for plain email messages.
    /// </summary>
    public string EmailQueueName { get; init; } = "email-queue";

    /// <summary>
    /// Gets or sets the queue name for templated email messages.
    /// </summary>
    public string TemplatedEmailQueueName { get; init; } = "templated-email-queue";

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL.
    /// </summary>
    public bool UseSsl { get; init; } = false;
}
