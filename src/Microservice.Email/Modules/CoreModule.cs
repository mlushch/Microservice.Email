using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Services;
using Microservice.Email.Core.Validators;
using Microservice.Email.Domain.Contracts;

namespace Microservice.Email.Modules;

/// <summary>
/// Module for registering core business logic services.
/// </summary>
public sealed class CoreModule : IModule
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register validators
        services.AddScoped<IValidator<SendEmailRequest>, SendEmailRequestValidator>();
        services.AddScoped<IValidator<SendTemplatedEmailRequest>, SendTemplatedEmailRequestValidator>();
        services.AddScoped<IValidator<CreateEmailTemplateRequest>, CreateEmailTemplateRequestValidator>();

        // Register services
        services.AddScoped<IEmailService, EmailService>();
    }
}
