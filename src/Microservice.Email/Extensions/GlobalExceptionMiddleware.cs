using System.Net;
using System.Text.Json;

using Microservice.Email.Core.Exceptions;

namespace Microservice.Email.Extensions;

/// <summary>
/// Middleware for handling exceptions globally.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<GlobalExceptionMiddleware> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionMiddleware"/> class.
    /// </summary>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            await this.HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Message = "Validation failed",
                    Errors = validationEx.Errors.Select(e => new ErrorDetail
                    {
                        Property = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                }),

            TemplateNotFoundException templateEx => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    Message = templateEx.Message
                }),

            FileStorageException fileEx => (
                HttpStatusCode.ServiceUnavailable,
                new ErrorResponse
                {
                    Message = "File storage service is unavailable",
                    Errors = new List<ErrorDetail>
                    {
                        new() { Property = "FileStorage", Message = fileEx.Message }
                    }
                }),

            EmailSendException emailEx => (
                HttpStatusCode.ServiceUnavailable,
                new ErrorResponse
                {
                    Message = "Email service is unavailable",
                    Errors = new List<ErrorDetail>
                    {
                        new() { Property = "Email", Message = emailEx.Message }
                    }
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Message = "An unexpected error occurred"
                })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            this.logger.LogError(exception, "Unhandled exception occurred");
        }
        else
        {
            this.logger.LogWarning(exception, "Handled exception occurred: {ExceptionType}", exception.GetType().Name);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Error response model.
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets or sets the detailed errors.
    /// </summary>
    public List<ErrorDetail>? Errors { get; init; }
}

/// <summary>
/// Error detail model.
/// </summary>
public sealed class ErrorDetail
{
    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string? Property { get; init; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public required string Message { get; init; }
}

/// <summary>
/// Extension methods for global exception handling.
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adds the global exception handling middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
