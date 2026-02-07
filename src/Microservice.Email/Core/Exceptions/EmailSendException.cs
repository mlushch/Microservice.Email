namespace Microservice.Email.Core.Exceptions;

/// <summary>
/// Exception thrown when email sending fails.
/// </summary>
public sealed class EmailSendException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSendException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public EmailSendException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSendException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public EmailSendException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
