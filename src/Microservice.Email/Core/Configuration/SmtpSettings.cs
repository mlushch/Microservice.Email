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
    public required string Host { get; init; }

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
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
    public string? DefaultSenderEmail { get; init; }

    /// <summary>
    /// Gets or sets the default sender name.
    /// </summary>
    public string? DefaultSenderName { get; init; }

    /// <summary>
    /// Gets or sets the maximum retry attempts for failed sends.
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Gets or sets the delay between retry attempts in milliseconds.
    /// </summary>
    public int RetryDelayMilliseconds { get; init; } = 1000;
}
