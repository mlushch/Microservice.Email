using Microservice.Email.Core.Validation;

namespace Microservice.Email.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationException(IReadOnlyList<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="propertyName">The property name that failed validation.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationException(string propertyName, string errorMessage)
        : base(errorMessage)
    {
        this.Errors = new[] { new ValidationError(propertyName, errorMessage) };
    }
}
