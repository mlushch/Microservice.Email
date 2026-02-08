using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "MinIO endpoint is required")]
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets or sets the MinIO access key.
    /// </summary>
    [Required(ErrorMessage = "MinIO access key is required")]
    public required string AccessKey { get; init; }

    /// <summary>
    /// Gets or sets the MinIO secret key.
    /// </summary>
    [Required(ErrorMessage = "MinIO secret key is required")]
    public required string SecretKey { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL.
    /// </summary>
    public bool UseSSL { get; init; } = false;

    /// <summary>
    /// Gets or sets the bucket name for email attachments.
    /// </summary>
    [Required(ErrorMessage = "Attachments bucket name is required")]
    [RegularExpression(@"^[a-z0-9][a-z0-9\-]{1,61}[a-z0-9]$", ErrorMessage = "Bucket name must be 3-63 characters, lowercase letters, numbers, and hyphens only")]
    public string AttachmentsBucket { get; init; } = "attachments";

    /// <summary>
    /// Gets or sets the bucket name for email templates.
    /// </summary>
    [Required(ErrorMessage = "Templates bucket name is required")]
    [RegularExpression(@"^[a-z0-9][a-z0-9\-]{1,61}[a-z0-9]$", ErrorMessage = "Bucket name must be 3-63 characters, lowercase letters, numbers, and hyphens only")]
    public string TemplatesBucket { get; init; } = "templates";
}
