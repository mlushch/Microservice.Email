using Prometheus;

namespace Microservice.Email.Core.Metrics;

/// <summary>
/// Prometheus metrics for email operations.
/// </summary>
public static class EmailMetrics
{
    /// <summary>
    /// Counter for total emails sent, labeled by status (success/failure).
    /// </summary>
    public static readonly Counter EmailsSentTotal = Prometheus.Metrics.CreateCounter(
        "email_sent_total",
        "Total number of emails sent",
        new CounterConfiguration
        {
            LabelNames = new[] { "status", "template" }
        });

    /// <summary>
    /// Counter for template operations, labeled by operation type.
    /// </summary>
    public static readonly Counter TemplateOperationsTotal = Prometheus.Metrics.CreateCounter(
        "email_template_operations_total",
        "Total number of template operations",
        new CounterConfiguration
        {
            LabelNames = new[] { "operation" }
        });

    /// <summary>
    /// Histogram for email send duration in seconds.
    /// </summary>
    public static readonly Histogram EmailSendDuration = Prometheus.Metrics.CreateHistogram(
        "email_send_duration_seconds",
        "Duration of email sending operations in seconds",
        new HistogramConfiguration
        {

            Buckets = new[] { 0.1, 0.25, 0.5, 1.0, 2.5, 5.0, 10.0 }
        });

    /// <summary>
    /// Gauge for RabbitMQ queue depth.
    /// </summary>
    public static readonly Gauge QueueDepth = Prometheus.Metrics.CreateGauge(
        "email_queue_depth",
        "Current depth of the email queue",
        new GaugeConfiguration
        {
            LabelNames = new[] { "queue_name" }
        });

    /// <summary>
    /// Counter for storage operations, labeled by operation type and status.
    /// </summary>
    public static readonly Counter StorageOperationsTotal = Prometheus.Metrics.CreateCounter(
        "email_storage_operations_total",
        "Total number of storage operations",
        new CounterConfiguration
        {
            LabelNames = new[] { "operation", "status" }
        });

    /// <summary>
    /// Counter for exceptions by type.
    /// </summary>
    public static readonly Counter ExceptionsTotal = Prometheus.Metrics.CreateCounter(
        "email_exceptions_total",
        "Total number of exceptions",
        new CounterConfiguration
        {
            LabelNames = new[] { "exception_type" }
        });

    /// <summary>
    /// Records a successful email send.
    /// </summary>
    /// <param name="templateName">The template name, or "plain" for non-templated emails.</param>
    /// <param name="duration">The duration of the send operation.</param>
    public static void RecordEmailSent(string templateName, TimeSpan duration)
    {
        EmailsSentTotal.WithLabels("success", templateName).Inc();
        EmailSendDuration.WithLabels(templateName).Observe(duration.TotalSeconds);
    }

    /// <summary>
    /// Records a failed email send.
    /// </summary>
    /// <param name="templateName">The template name, or "plain" for non-templated emails.</param>
    public static void RecordEmailFailed(string templateName)
    {
        EmailsSentTotal.WithLabels("failure", templateName).Inc();
    }

    /// <summary>
    /// Records a template operation.
    /// </summary>
    /// <param name="operation">The operation type (create, delete, get).</param>
    public static void RecordTemplateOperation(string operation)
    {
        TemplateOperationsTotal.WithLabels(operation).Inc();
    }

    /// <summary>
    /// Records a storage operation.
    /// </summary>
    /// <param name="operation">The operation type (upload, download, delete).</param>
    /// <param name="success">Whether the operation was successful.</param>
    public static void RecordStorageOperation(string operation, bool success)
    {
        StorageOperationsTotal.WithLabels(operation, success ? "success" : "failure").Inc();
    }

    /// <summary>
    /// Records an exception.
    /// </summary>
    /// <param name="exceptionType">The type of exception.</param>
    public static void RecordException(string exceptionType)
    {
        ExceptionsTotal.WithLabels(exceptionType).Inc();
    }
}
