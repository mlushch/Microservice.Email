using Microsoft.EntityFrameworkCore;

using Microservice.Email.Core.Configuration;
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
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
        
        // Use Database settings if available, otherwise fall back to ConnectionStrings
        var connectionString = databaseSettings?.DefaultConnection 
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EmailDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(databaseSettings?.CommandTimeout ?? 30);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: databaseSettings?.MaxRetryCount ?? 3,
                    maxRetryDelay: TimeSpan.FromSeconds(databaseSettings?.MaxRetryDelay ?? 30),
                    errorCodesToAdd: null);
            })
            .UseSnakeCaseNamingConvention();

            if (databaseSettings?.EnableSensitiveDataLogging == true)
            {
                options.EnableSensitiveDataLogging();
            }

            if (databaseSettings?.EnableDetailedErrors == true)
            {
                options.EnableDetailedErrors();
            }
        });
    }
}
