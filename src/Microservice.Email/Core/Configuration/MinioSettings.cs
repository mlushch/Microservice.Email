namespace Microservice.Email.Core.Configuration;

/// <summary>
/// Configuration settings for MinIO file storage.
/// </summary>
public sealed class MinioSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Minio";

    /// <summary>
    /// Gets or sets the MinIO server endpoint.
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets or sets the MinIO access key.
    /// </summary>
    public required string AccessKey { get; init; }

    /// <summary>
    /// Gets or sets the MinIO secret key.
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL.
    /// </summary>
    public bool UseSSL { get; init; } = false;

    /// <summary>
    /// Gets or sets the bucket name for email attachments.
    /// </summary>
    public string AttachmentsBucket { get; init; } = "attachments";

    /// <summary>
    /// Gets or sets the bucket name for email templates.
    /// </summary>
    public string TemplatesBucket { get; init; } = "templates";
}
