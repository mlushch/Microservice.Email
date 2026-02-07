using System.Text.RegularExpressions;

using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Validation;
using Microservice.Email.Domain.Contracts;

namespace Microservice.Email.Core.Validators;

/// <summary>
/// Validator for SendTemplatedEmailRequest.
/// </summary>
public sealed partial class SendTemplatedEmailRequestValidator : IValidator<SendTemplatedEmailRequest>
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    /// <inheritdoc />
    public ValidationResult Validate(SendTemplatedEmailRequest instance)
    {
        var result = new ValidationResult();

        if (instance.Sender is null)
        {
            result.AddError(nameof(instance.Sender), "Sender is required.");
        }
        else if (string.IsNullOrWhiteSpace(instance.Sender.Email))
        {
            result.AddError(nameof(instance.Sender.Email), "Sender email is required.");
        }
        else if (!EmailRegex().IsMatch(instance.Sender.Email))
        {
            result.AddError(nameof(instance.Sender.Email), "Sender email is not valid.");
        }

        if (instance.Recipients is null || instance.Recipients.Length == 0)
        {
            result.AddError(nameof(instance.Recipients), "At least one recipient is required.");
        }
        else
        {
            for (int i = 0; i < instance.Recipients.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(instance.Recipients[i]))
                {
                    result.AddError($"{nameof(instance.Recipients)}[{i}]", "Recipient email cannot be empty.");
                }
                else if (!EmailRegex().IsMatch(instance.Recipients[i]))
                {
                    result.AddError($"{nameof(instance.Recipients)}[{i}]", $"Recipient email '{instance.Recipients[i]}' is not valid.");
                }
            }
        }

        if (string.IsNullOrWhiteSpace(instance.TemplateName))
        {
            result.AddError(nameof(instance.TemplateName), "Template name is required.");
        }

        if (instance.TemplateProperties is null)
        {
            result.AddError(nameof(instance.TemplateProperties), "Template properties are required.");
        }

        return result;
    }
}
