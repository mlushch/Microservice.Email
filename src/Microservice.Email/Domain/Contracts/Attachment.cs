namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Represents an email attachment with base64-encoded content.
/// </summary>
public sealed class Attachment
{
    /// <summary>
    /// Gets the file name of the attachment.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the base64-encoded content of the attachment file.
    /// </summary>
    public required string ContentBase64 { get; init; }

    /// <summary>
    /// Gets the MIME content type of the attachment. This is optional.
    /// </summary>
    public string? ContentType { get; init; }
}
