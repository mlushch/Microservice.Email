namespace Microservice.Email.Core.Exceptions;

/// <summary>
/// Exception thrown when email sending fails.
/// </summary>
public sealed class EmailSendException : Exception
{
    public EmailSendException(string message)
        : base(message)
    {
    }

    public EmailSendException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
