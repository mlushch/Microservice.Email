using System.ComponentModel.DataAnnotations;

namespace Microservice.Email.Core.Configuration;

/// <summary>
/// Configuration settings for Prometheus metrics.
/// </summary>
public sealed class PrometheusSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Prometheus";

    /// <summary>
    /// Gets or sets a value indicating whether Prometheus metrics are enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets or sets the metrics endpoint path.
    /// </summary>
    [Required(ErrorMessage = "Metrics endpoint is required when Prometheus is enabled")]
    public string MetricsEndpoint { get; init; } = "/metrics";

    /// <summary>
    /// Gets or sets the allowed hosts for the metrics endpoint.
    /// Use "*" to allow all hosts.
    /// </summary>
    public string[] AllowedHosts { get; init; } = ["localhost", "127.0.0.1"];

    /// <summary>
    /// Gets or sets a value indicating whether to include HTTP request metrics.
    /// </summary>
    public bool IncludeHttpMetrics { get; init; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to include custom host labels.
    /// </summary>
    public bool IncludeHostLabel { get; init; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to suppress default metrics.
    /// </summary>
    public bool SuppressDefaultMetrics { get; init; } = false;
}
