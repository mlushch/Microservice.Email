namespace Microservice.Email.Modules;

/// <summary>
/// Interface for dependency injection modules.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Registers services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}
