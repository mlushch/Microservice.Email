namespace Microservice.Email.Core.Validation;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public sealed class ValidationResult
{
    private readonly List<ValidationError> errors = new();

    /// <summary>
    /// Gets a value indicating whether the validation passed.
    /// </summary>
    public bool IsValid => this.errors.Count == 0;

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => this.errors.AsReadOnly();

    /// <summary>
    /// Adds a validation error.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorMessage">The error message.</param>
    public void AddError(string propertyName, string errorMessage)
    {
        this.errors.Add(new ValidationError(propertyName, errorMessage));
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A valid validation result.</returns>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with a single error.
    /// </summary>
    /// <param name="propertyName">The property name that failed.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>An invalid validation result.</returns>
    public static ValidationResult Failure(string propertyName, string errorMessage)
    {
        var result = new ValidationResult();
        result.AddError(propertyName, errorMessage);
        return result;
    }
}
