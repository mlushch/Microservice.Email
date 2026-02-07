using Microsoft.AspNetCore.Http;

namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Represents a request to create a new email template.
/// </summary>
public sealed class CreateEmailTemplateRequest
{
    /// <summary>
    /// Gets the unique name for the email template.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the storage path for the email template.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Gets the template file to upload.
    /// </summary>
    public required IFormFile File { get; init; }
}
