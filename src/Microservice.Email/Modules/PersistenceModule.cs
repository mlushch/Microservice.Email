using Microsoft.EntityFrameworkCore;

using Microservice.Email.Infrastructure.Persistence;

namespace Microservice.Email.Modules;

/// <summary>
/// Module for registering persistence-related services.
/// </summary>
public sealed class PersistenceModule : IModule
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EmailDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }
}
