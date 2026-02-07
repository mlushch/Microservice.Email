namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Wraps an email request with optional base64-encoded attachments.
/// </summary>
/// <typeparam name="T">The type of the email request being wrapped.</typeparam>
public sealed class AttachmentsWrapper<T>
{
    /// <summary>
    /// Gets the email request payload.
    /// </summary>
    public required T Email { get; init; }

    /// <summary>
    /// Gets the array of attachments to include with the email.
    /// </summary>
    public Attachment[]? Attachments { get; init; }
}
