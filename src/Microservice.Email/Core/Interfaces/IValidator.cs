using Microservice.Email.Core.Validation;

namespace Microservice.Email.Core.Interfaces;

/// <summary>
/// Interface for validating objects of type T.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified instance.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>The validation result containing any errors.</returns>
    ValidationResult Validate(T instance);
}
