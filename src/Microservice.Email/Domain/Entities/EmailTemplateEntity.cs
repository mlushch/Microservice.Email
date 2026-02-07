namespace Microservice.Email.Domain.Entities;

/// <summary>
/// Represents an email template entity in the database.
/// </summary>
public sealed class EmailTemplateEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the template.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the path to the template file in storage.
    /// </summary>
    public required string Path { get; set; }

    /// <summary>
    /// Gets or sets the unique name of the template.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the size of the template in bytes.
    /// </summary>
    public int Size { get; set; }
}
