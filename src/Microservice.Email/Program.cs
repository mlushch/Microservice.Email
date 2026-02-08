using Microservice.Email.Extensions;
using Microservice.Email.Modules;

using Prometheus;

using Serilog;
using Serilog.Events;

// Configure Serilog bootstrap logger for startup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Microservice.Email application");

    var builder = WebApplication.CreateBuilder(args);

    // Add additional configuration sources
    // JSON configuration is added by default
    // Environment variables with "EMAIL_" prefix are mapped to configuration
    builder.Configuration
        .AddEnvironmentVariables("EMAIL_")
        .AddUserSecrets<Program>(optional: true);

    // Configure Serilog from configuration
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "Microservice.Email"));

    // Add configuration with validation
    builder.Services.AddApplicationConfiguration(builder.Configuration);

    // Add services using modules
    builder.Services.AddModules(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks();

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Microservice.Email API",
        Version = "v1",
        Description = "Email delivery microservice with template support. Provides REST API for sending emails, managing templates, and tracking delivery status.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add common response types documentation
    options.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace("+", "."));
});

    var app = builder.Build();

    // Add correlation ID middleware for request tracing
    app.UseCorrelationId();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        };
    });

    // Configure the HTTP request pipeline.
    // Enable Prometheus HTTP request metrics
    app.UseHttpMetrics(options =>
    {
        options.AddCustomLabel("host", context => context.Request.Host.Host);
    });

    // Configure the HTTP request pipeline.
    app.UseGlobalExceptionHandler();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservice.Email API v1");
            options.DisplayRequestDuration();
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    // Map health check endpoint
    app.MapHealthChecks("/health");

    // Map Prometheus metrics endpoint with restricted host access
    var metricsEndpoint = app.MapMetrics();
    metricsEndpoint.RequireHost("localhost", "127.0.0.1", "[::1]");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Shutting down Microservice.Email application");
    Log.CloseAndFlush();
}