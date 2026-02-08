using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Metrics;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Entities;
using Microservice.Email.Domain.Models;
using Microservice.Email.Infrastructure.Persistence;

using Scriban;

namespace Microservice.Email.Core.Services;

/// <summary>
/// Service for email template operations using Scriban templating engine.
/// </summary>
public sealed class TemplateService : ITemplateService
{
    private readonly EmailDbContext dbContext;
    private readonly IFileStorageService fileStorageService;
    private readonly IValidator<CreateEmailTemplateRequest> validator;
    private readonly IMemoryCache cache;
    private readonly MinioSettings minioSettings;
    private readonly ILogger<TemplateService> logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public TemplateService(
        EmailDbContext dbContext,
        IFileStorageService fileStorageService,
        IValidator<CreateEmailTemplateRequest> validator,
        IMemoryCache cache,
        IOptions<MinioSettings> minioSettings,
        ILogger<TemplateService> logger)
    {
        this.dbContext = dbContext;
        this.fileStorageService = fileStorageService;
        this.validator = validator;
        this.cache = cache;
        this.minioSettings = minioSettings.Value;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmailTemplateResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EmailMetrics.RecordTemplateOperation("get_all");

        var templates = await dbContext.EmailTemplates
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new EmailTemplateResponse
            {
                Id = t.Id,
                Name = t.Name,
                Path = t.Path,
                Size = t.Size
            })
            .ToListAsync(cancellationToken);

        return templates;
    }

    /// <inheritdoc />
    public async Task CreateAsync(CreateEmailTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingTemplate = await dbContext.EmailTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == request.Name, cancellationToken);

        if (existingTemplate is not null)
        {
            throw new ValidationException(nameof(request.Name), $"Template with name '{request.Name}' already exists.");
        }

        using var stream = request.File.OpenReadStream();

        // Validate Scriban template syntax
        using var reader = new StreamReader(stream);
        var templateContent = await reader.ReadToEndAsync(cancellationToken);
        ValidateTemplate(templateContent, request.Name);

        // Reset stream position for upload
        stream.Position = 0;

        var fileName = $"{request.Name}{Path.GetExtension(request.File.FileName)}";
        await fileStorageService.UploadAsync(
            stream,
            fileName,
            minioSettings.TemplatesBucket,
            "text/html",
            cancellationToken);

        var templateEntity = new EmailTemplateEntity
        {
            Name = request.Name,
            Path = $"{minioSettings.TemplatesBucket}/{fileName}",
            Size = (int)request.File.Length
        };

        dbContext.EmailTemplates.Add(templateEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        EmailMetrics.RecordTemplateOperation("create");

        logger.LogInformation("Created email template {TemplateName}", request.Name);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int templateId, CancellationToken cancellationToken = default)
    {
        var template = await dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

        if (template is null)
        {
            throw new TemplateNotFoundException($"Template with ID {templateId}");
        }

        var fileName = Path.GetFileName(template.Path);
        await fileStorageService.RemoveAsync(
            fileName,
            minioSettings.TemplatesBucket,
            cancellationToken);

        dbContext.EmailTemplates.Remove(template);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        cache.Remove(GetCacheKey(template.Name));

        EmailMetrics.RecordTemplateOperation("delete");

        logger.LogInformation("Deleted email template {TemplateName}", template.Name);
    }

    /// <inheritdoc />
    public async Task<string> RenderAsync(string templateName, Dictionary<string, object> properties, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(templateName);

        if (!cache.TryGetValue(cacheKey, out Template? template))
        {
            var templateEntity = await dbContext.EmailTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == templateName, cancellationToken);

            if (templateEntity is null)
            {
                throw new TemplateNotFoundException(templateName);
            }

            var fileName = Path.GetFileName(templateEntity.Path);
            using var stream = await fileStorageService.DownloadAsync(
                fileName,
                minioSettings.TemplatesBucket,
                cancellationToken);

            using var reader = new StreamReader(stream);
            var templateContent = await reader.ReadToEndAsync(cancellationToken);

            template = Template.Parse(templateContent);

            if (template.HasErrors)
            {
                var errors = string.Join(", ", template.Messages.Select(m => m.Message));
                throw new ValidationException("TemplateContent", $"Template '{templateName}' has syntax errors: {errors}");
            }

            cache.Set(cacheKey, template, CacheDuration);
            logger.LogDebug("Cached template {TemplateName}", templateName);
        }

        try
        {
            var result = await template!.RenderAsync(properties);
            logger.LogDebug("Rendered template {TemplateName}", templateName);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to render template {TemplateName}", templateName);
            throw new ValidationException("TemplateContent", $"Failed to render template '{templateName}': {ex.Message}");
        }
    }

    private static void ValidateTemplate(string templateContent, string templateName)
    {
        var template = Template.Parse(templateContent);

        if (template.HasErrors)
        {
            var errors = string.Join(", ", template.Messages.Select(m => m.Message));
            throw new ValidationException("TemplateContent", $"Template '{templateName}' has syntax errors: {errors}");
        }
    }

    private static string GetCacheKey(string templateName) => $"email_template_{templateName}";
}
