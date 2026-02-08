using System.ComponentModel.DataAnnotations;

namespace Microservice.Email.Core.Configuration;

/// <summary>
/// Configuration settings for SMTP email delivery.
/// </summary>
public sealed class SmtpSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Smtp";

    /// <summary>
    /// Gets or sets the SMTP server host.
    /// </summary>
    [Required(ErrorMessage = "SMTP host is required")]
    public required string Host { get; init; }

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
    [Range(1, 65535, ErrorMessage = "SMTP port must be between 1 and 65535")]
    public int Port { get; init; } = 587;

    /// <summary>
    /// Gets or sets the SMTP username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Gets or sets the SMTP password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL/TLS.
    /// </summary>
    public bool EnableSsl { get; init; } = true;

    /// <summary>
    /// Gets or sets the default sender email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Default sender email must be a valid email address")]
    public string? DefaultSenderEmail { get; init; }

    /// <summary>
    /// Gets or sets the default sender name.
    /// </summary>
    public string? DefaultSenderName { get; init; }

    /// <summary>
    /// Gets or sets the maximum retry attempts for failed sends.
    /// </summary>
    [Range(0, 10, ErrorMessage = "Max retry attempts must be between 0 and 10")]
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Gets or sets the delay between retry attempts in milliseconds.
    /// </summary>
    [Range(100, 60000, ErrorMessage = "Retry delay must be between 100 and 60000 milliseconds")]
    public int RetryDelayMilliseconds { get; init; } = 1000;
}
