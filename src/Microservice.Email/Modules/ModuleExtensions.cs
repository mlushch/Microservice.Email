using System.Reflection;

namespace Microservice.Email.Modules;

/// <summary>
/// Extension methods for module registration.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Registers all modules found in the assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var moduleType in moduleTypes)
        {
            var module = (IModule?)Activator.CreateInstance(moduleType);
            module?.RegisterServices(services, configuration);
        }

        return services;
    }
}
