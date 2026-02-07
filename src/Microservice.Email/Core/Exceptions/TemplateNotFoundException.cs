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

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateNotFoundException"/> class.
    /// </summary>
    /// <param name="templateName">The name of the template that was not found.</param>
    public TemplateNotFoundException(string templateName)
        : base($"Email template '{templateName}' was not found.")
    {
        this.TemplateName = templateName;
    }
}
