using System.ComponentModel.DataAnnotations;

namespace Microservice.Email.Core.Configuration;

/// <summary>
/// Configuration settings for database connections.
/// </summary>
public sealed class DatabaseSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// Gets or sets the default connection string.
    /// </summary>
    [Required(ErrorMessage = "Database connection string is required")]
    public required string DefaultConnection { get; init; }

    /// <summary>
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    [Range(1, 600, ErrorMessage = "Command timeout must be between 1 and 600 seconds")]
    public int CommandTimeout { get; init; } = 30;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for transient failures.
    /// </summary>
    [Range(0, 10, ErrorMessage = "Max retry attempts must be between 0 and 10")]
    public int MaxRetryCount { get; init; } = 3;

    /// <summary>
    /// Gets or sets the maximum retry delay in seconds.
    /// </summary>
    [Range(1, 60, ErrorMessage = "Max retry delay must be between 1 and 60 seconds")]
    public int MaxRetryDelay { get; init; } = 30;

    /// <summary>
    /// Gets or sets a value indicating whether to enable sensitive data logging.
    /// Should only be enabled in development environments.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; init; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to enable detailed errors.
    /// </summary>
    public bool EnableDetailedErrors { get; init; } = false;
}
