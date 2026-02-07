namespace Microservice.Email.Domain.Entities;

/// <summary>
/// Represents an email message entity in the database.
/// </summary>
public sealed class EmailEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the email.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the email body content.
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// Gets or sets the email subject line.
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// Gets or sets the sender's display name.
    /// </summary>
    public required string SenderName { get; set; }

    /// <summary>
    /// Gets or sets the sender's email address.
    /// </summary>
    public required string SenderEmail { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the email was sent.
    /// </summary>
    public DateTimeOffset SentDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of the email.
    /// </summary>
    public EmailStatus EmailStatus { get; set; } = EmailStatus.Pending;

    /// <summary>
    /// Gets or sets the collection of recipients for this email.
    /// </summary>
    public ICollection<RecipientEntity> Recipients { get; set; } = new List<RecipientEntity>();
}
