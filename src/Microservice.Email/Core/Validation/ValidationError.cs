namespace Microservice.Email.Core.Validation;

/// <summary>
/// Represents a single validation error.
/// </summary>
/// <param name="PropertyName">The name of the property that failed validation.</param>
/// <param name="ErrorMessage">The validation error message.</param>
public sealed record ValidationError(string PropertyName, string ErrorMessage);
