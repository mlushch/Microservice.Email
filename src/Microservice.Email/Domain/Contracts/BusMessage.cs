namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Wraps a message payload for RabbitMQ messaging with a correlation identifier.
/// </summary>
/// <typeparam name="T">The type of the message payload.</typeparam>
public sealed class BusMessage<T>
{
    /// <summary>
    /// Gets the message payload.
    /// </summary>
    public required T Payload { get; init; }

    /// <summary>
    /// Gets the correlation identifier for tracking message flows.
    /// </summary>
    public required Guid CorrelationId { get; init; }
}
