using Microsoft.AspNetCore.Http;

namespace Microservice.Email.Domain.Contracts;

/// <summary>
/// Wraps an email request with optional file attachments uploaded via multipart/form-data.
/// </summary>
/// <typeparam name="T">The type of the email request being wrapped.</typeparam>
public sealed class FormFilesWrapper<T>
{
    /// <summary>
    /// Gets the email request payload.
    /// </summary>
    public required T Email { get; init; }

    /// <summary>
    /// Gets the array of files to attach to the email.
    /// </summary>
    public IFormFile[]? Files { get; init; }
}
