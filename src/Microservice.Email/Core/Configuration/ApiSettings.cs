using System.ComponentModel.DataAnnotations;

namespace Microservice.Email.Core.Configuration;

/// <summary>
/// Configuration settings for API and frontend integration.
/// </summary>
public sealed class ApiSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Api";

    /// <summary>
    /// Gets or sets the base URL for the API.
    /// </summary>
    [Required(ErrorMessage = "API base URL is required")]
    [Url(ErrorMessage = "API base URL must be a valid URL")]
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Gets or sets the allowed CORS origins.
    /// </summary>
    public string[] CorsOrigins { get; init; } = [];

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string Version { get; init; } = "v1";

    /// <summary>
    /// Gets or sets a value indicating whether Swagger documentation is enabled.
    /// </summary>
    public bool EnableSwagger { get; init; } = true;

    /// <summary>
    /// Gets or sets the request timeout in seconds.
    /// </summary>
    [Range(1, 300, ErrorMessage = "Request timeout must be between 1 and 300 seconds")]
    public int RequestTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Gets or sets the maximum request body size in bytes.
    /// Default is 10MB.
    /// </summary>
    [Range(1024, 104857600, ErrorMessage = "Max request body size must be between 1KB and 100MB")]
    public long MaxRequestBodySize { get; init; } = 10485760;

    /// <summary>
    /// Gets or sets a value indicating whether to enable detailed error responses.
    /// Should be false in production.
    /// </summary>
    public bool EnableDetailedErrors { get; init; } = false;
}
