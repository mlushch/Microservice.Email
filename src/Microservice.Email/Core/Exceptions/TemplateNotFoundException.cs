namespace Microservice.Email.Core.Exceptions;

/// <summary>
/// Exception thrown when an email template is not found.
/// </summary>
public sealed class TemplateNotFoundException : Exception
{
    /// <summary>
    /// Gets the template name that was not found.
    /// </summary>
    public string TemplateName { get; }


    public TemplateNotFoundException(string templateName)
        : base($"Email template '{templateName}' was not found.")
    {
        this.TemplateName = templateName;
    }
}
