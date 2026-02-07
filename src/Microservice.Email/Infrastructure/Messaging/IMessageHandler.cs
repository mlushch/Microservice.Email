using Microservice.Email.Domain.Contracts;

namespace Microservice.Email.Infrastructure.Messaging;

/// <summary>
/// Interface for handling messages of type T from the message bus.
/// </summary>
/// <typeparam name="T">The type of message payload to handle.</typeparam>
public interface IMessageHandler<T>
{
    /// <summary>
    /// Handles the incoming message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task HandleAsync(BusMessage<T> message, CancellationToken cancellationToken = default);
}
