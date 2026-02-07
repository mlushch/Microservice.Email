namespace Microservice.Email.Domain.Entities;

/// <summary>
/// Represents an email recipient entity in the database.
/// </summary>
public sealed class RecipientEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the recipient.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the recipient's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the foreign key reference to the email.
    /// </summary>
    public int EmailId { get; set; }

    /// <summary>
    /// Gets or sets the associated email entity.
    /// </summary>
    public required EmailEntity Email_Navigation { get; set; }
}
