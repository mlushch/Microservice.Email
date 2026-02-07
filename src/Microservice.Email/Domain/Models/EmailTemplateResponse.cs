namespace Microservice.Email.Domain.Models;

/// <summary>
/// Represents the response model for an email template.
/// </summary>
public sealed class EmailTemplateResponse
{
    /// <summary>
    /// Gets the unique identifier for the template.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the unique name of the template.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the path to the template file in storage.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Gets the size of the template in bytes.
    /// </summary>
    public required int Size { get; init; }
}
