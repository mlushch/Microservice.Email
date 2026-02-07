using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Validation;
using Microservice.Email.Domain.Contracts;

namespace Microservice.Email.Core.Validators;

/// <summary>
/// Validator for CreateEmailTemplateRequest.
/// </summary>
public sealed class CreateEmailTemplateRequestValidator : IValidator<CreateEmailTemplateRequest>
{
    private static readonly string[] AllowedExtensions = { ".html", ".htm" };

    /// <inheritdoc />
    public ValidationResult Validate(CreateEmailTemplateRequest instance)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            result.AddError(nameof(instance.Name), "Template name is required.");
        }

        if (string.IsNullOrWhiteSpace(instance.Path))
        {
            result.AddError(nameof(instance.Path), "Template path is required.");
        }

        if (instance.File is null)
        {
            result.AddError(nameof(instance.File), "Template file is required.");
        }
        else
        {
            var extension = System.IO.Path.GetExtension(instance.File.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                result.AddError(nameof(instance.File), "Template file must be an HTML file (.html or .htm).");
            }

            if (instance.File.Length == 0)
            {
                result.AddError(nameof(instance.File), "Template file cannot be empty.");
            }
        }

        return result;
    }
}
