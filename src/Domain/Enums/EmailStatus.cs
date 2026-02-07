namespace Microservice.Email.Domain.Enums;

/// <summary>
/// Represents the status of an email message.
/// </summary>
public enum EmailStatus
{
    /// <summary>
    /// Email is waiting to be sent.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Email has been successfully sent.
    /// </summary>
    Sent = 1,

    /// <summary>
    /// Email failed to send.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Email is currently being sent.
    /// </summary>
    Sending = 3
}
