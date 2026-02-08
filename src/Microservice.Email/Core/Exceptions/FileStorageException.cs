namespace Microservice.Email.Core.Exceptions;

/// <summary>
/// Exception thrown when file storage operations fail.
/// </summary>
public sealed class FileStorageException : Exception
{
    public FileStorageException(string message)
        : base(message)
    {
    }

    public FileStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
