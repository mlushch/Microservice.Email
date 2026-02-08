using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

using Microservice.Email.Core.Configuration;

namespace Microservice.Email.Extensions;

/// <summary>
/// Extension methods for configuration management.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds and validates all application configuration sections.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database settings
        services.AddOptionsWithValidation<DatabaseSettings>(
            configuration,
            DatabaseSettings.SectionName);

        // MinIO settings
        services.AddOptionsWithValidation<MinioSettings>(
            configuration,
            MinioSettings.SectionName);

        // SMTP settings
        services.AddOptionsWithValidation<SmtpSettings>(
            configuration,
            SmtpSettings.SectionName);

        // RabbitMQ settings
        services.AddOptionsWithValidation<RabbitMqSettings>(
            configuration,
            RabbitMqSettings.SectionName);

        // Prometheus settings
        services.AddOptionsWithValidation<PrometheusSettings>(
            configuration,
            PrometheusSettings.SectionName);

        // API settings
        services.AddOptionsWithValidation<ApiSettings>(
            configuration,
            ApiSettings.SectionName);

        return services;
    }

    /// <summary>
    /// Adds options with data annotation validation and validates on startup.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The options builder for additional configuration.</returns>
    public static OptionsBuilder<TOptions> AddOptionsWithValidation<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        return services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    /// <summary>
    /// Gets a required configuration value.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration value is missing.</exception>
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' is required but was not found.");
        }
        return value;
    }

    /// <summary>
    /// Gets a required configuration section.
    /// </summary>
    /// <typeparam name="T">The type to bind the section to.</typeparam>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The section name.</param>
    /// <returns>The bound configuration section.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the section is missing or invalid.</exception>
    public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName)
        where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' is required but was not found.");
        }

        var settings = section.Get<T>();
        if (settings is null)
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' could not be bound to type {typeof(T).Name}.");
        }

        // Validate data annotations
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(settings);
        if (!Validator.TryValidateObject(settings, validationContext, validationResults, validateAllProperties: true))
        {
            var errors = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Configuration section '{sectionName}' failed validation:{Environment.NewLine}{errors}");
        }

        return settings;
    }
}
