using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Services;

namespace Microservice.Email.Modules;

/// <summary>
/// Module for registering template-related services.
/// </summary>
public sealed class TemplateModule : IModule
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register memory cache for template caching
        services.AddMemoryCache();

        // Register template service
        services.AddScoped<ITemplateService, TemplateService>();
    }
}
