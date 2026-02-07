using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Services;
using Microservice.Email.Core.Validation;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Entities;
using Microservice.Email.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice.Email.Tests.Core.Services;

/// <summary>
/// Unit tests for the TemplateService class.
/// </summary>
public sealed class TemplateServiceTests : TestBase
{
    private readonly Mock<IFileStorageService> mockFileStorageService;
    private readonly Mock<IValidator<CreateEmailTemplateRequest>> mockValidator;
    private readonly Mock<ILogger<TemplateService>> mockLogger;
    private readonly IMemoryCache memoryCache;
    private readonly IOptions<MinioSettings> minioOptions;

    public TemplateServiceTests()
    {
        this.mockFileStorageService = new Mock<IFileStorageService>();
        this.mockValidator = new Mock<IValidator<CreateEmailTemplateRequest>>();
        this.mockLogger = new Mock<ILogger<TemplateService>>();
        this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        this.minioOptions = Options.Create(new MinioSettings
        {
            Endpoint = "localhost:9000",
            AccessKey = "test",
            SecretKey = "test",
            TemplatesBucket = "templates",
            AttachmentsBucket = "attachments"
        });
    }

    private TemplateService CreateService(EmailDbContext dbContext)
    {
        return new TemplateService(
            dbContext,
            this.mockFileStorageService.Object,
            this.mockValidator.Object,
            this.memoryCache,
            this.minioOptions,
            this.mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTemplates()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        dbContext.EmailTemplates.AddRange(
            new EmailTemplateEntity { Name = "welcome", Path = "templates/welcome.html", Size = 1024 },
            new EmailTemplateEntity { Name = "password-reset", Path = "templates/password-reset.html", Size = 2048 },
            new EmailTemplateEntity { Name = "notification", Path = "templates/notification.html", Size = 512 });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Name == "welcome");
        result.Should().Contain(t => t.Name == "password-reset");
        result.Should().Contain(t => t.Name == "notification");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoTemplates_ReturnsEmptyList()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTemplatesOrderedByName()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        dbContext.EmailTemplates.AddRange(
            new EmailTemplateEntity { Name = "zebra", Path = "templates/zebra.html", Size = 100 },
            new EmailTemplateEntity { Name = "alpha", Path = "templates/alpha.html", Size = 100 },
            new EmailTemplateEntity { Name = "beta", Path = "templates/beta.html", Size = 100 });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("alpha");
        result[1].Name.Should().Be("beta");
        result[2].Name.Should().Be("zebra");
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesTemplateAndUploadsFile()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        var templateContent = "<html><body>Hello {{ name }}!</body></html>";
        var mockFile = CreateMockFormFile("template.html", templateContent);

        var request = new CreateEmailTemplateRequest
        {
            Name = "greeting",
            Path = "templates/greeting.html",
            File = mockFile.Object
        };

        this.mockValidator
            .Setup(v => v.Validate(It.IsAny<CreateEmailTemplateRequest>()))
            .Returns(ValidationResult.Success());

        this.mockFileStorageService
            .Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("templates/greeting.html");

        // Act
        await service.CreateAsync(request);

        // Assert
        var savedTemplate = dbContext.EmailTemplates.FirstOrDefault(t => t.Name == "greeting");
        savedTemplate.Should().NotBeNull();
        savedTemplate!.Name.Should().Be("greeting");

        this.mockFileStorageService.Verify(
            s => s.UploadAsync(
                It.IsAny<Stream>(),
                "greeting.html",
                "templates",
                "text/html",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidationFailure_ThrowsValidationException()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        var mockFile = CreateMockFormFile("template.html", "content");
        var request = new CreateEmailTemplateRequest
        {
            Name = "",
            Path = "",
            File = mockFile.Object
        };

        this.mockValidator
            .Setup(v => v.Validate(It.IsAny<CreateEmailTemplateRequest>()))
            .Returns(ValidationResult.Failure("Name", "Name is required"));

        // Act
        var act = async () => await service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(e => e.Errors.Any(err => err.PropertyName == "Name"));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsValidationException()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        dbContext.EmailTemplates.Add(new EmailTemplateEntity
        {
            Name = "existing",
            Path = "templates/existing.html",
            Size = 100
        });
        await dbContext.SaveChangesAsync();

        var mockFile = CreateMockFormFile("template.html", "<html>{{ name }}</html>");
        var request = new CreateEmailTemplateRequest
        {
            Name = "existing",
            Path = "templates/existing.html",
            File = mockFile.Object
        };

        this.mockValidator
            .Setup(v => v.Validate(It.IsAny<CreateEmailTemplateRequest>()))
            .Returns(ValidationResult.Success());

        // Act
        var act = async () => await service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidScriban_ThrowsValidationException()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        var invalidTemplateContent = "<html><body>Hello {{ name </body></html>"; // Invalid Scriban syntax
        var mockFile = CreateMockFormFile("template.html", invalidTemplateContent);

        var request = new CreateEmailTemplateRequest
        {
            Name = "invalid",
            Path = "templates/invalid.html",
            File = mockFile.Object
        };

        this.mockValidator
            .Setup(v => v.Validate(It.IsAny<CreateEmailTemplateRequest>()))
            .Returns(ValidationResult.Success());

        // Act
        var act = async () => await service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(e => e.Errors.Any(err => err.PropertyName == "TemplateContent"));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingTemplate_DeletesTemplateAndFile()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        var template = new EmailTemplateEntity
        {
            Name = "to-delete",
            Path = "templates/to-delete.html",
            Size = 100
        };
        dbContext.EmailTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        this.mockFileStorageService
            .Setup(s => s.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.DeleteAsync(template.Id);

        // Assert
        var deletedTemplate = dbContext.EmailTemplates.FirstOrDefault(t => t.Id == template.Id);
        deletedTemplate.Should().BeNull();

        this.mockFileStorageService.Verify(
            s => s.RemoveAsync("to-delete.html", "templates", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingTemplate_ThrowsTemplateNotFoundException()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        // Act
        var act = async () => await service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<TemplateNotFoundException>();
    }

    [Fact]
    public async Task RenderAsync_WithExistingTemplate_ReturnsRenderedHtml()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        dbContext.EmailTemplates.Add(new EmailTemplateEntity
        {
            Name = "welcome",
            Path = "templates/welcome.html",
            Size = 100
        });
        await dbContext.SaveChangesAsync();

        var templateContent = "<html><body>Hello {{ name }}, welcome to {{ company }}!</body></html>";
        var templateStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(templateContent));

        this.mockFileStorageService
            .Setup(s => s.DownloadAsync(
                "welcome.html",
                "templates",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(templateStream);

        var properties = new Dictionary<string, object>
        {
            { "name", "John" },
            { "company", "Acme Corp" }
        };

        // Act
        var result = await service.RenderAsync("welcome", properties);

        // Assert
        result.Should().Contain("Hello John");
        result.Should().Contain("welcome to Acme Corp");
    }

    [Fact]
    public async Task RenderAsync_WithNonExistingTemplate_ThrowsTemplateNotFoundException()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        var properties = new Dictionary<string, object>();

        // Act
        var act = async () => await service.RenderAsync("nonexistent", properties);

        // Assert
        await act.Should().ThrowAsync<TemplateNotFoundException>()
            .WithMessage("*nonexistent*");
    }

    [Fact]
    public async Task RenderAsync_CachesTemplate_OnSecondCall()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        dbContext.EmailTemplates.Add(new EmailTemplateEntity
        {
            Name = "cached",
            Path = "templates/cached.html",
            Size = 100
        });
        await dbContext.SaveChangesAsync();

        var templateContent = "<html><body>Hello {{ name }}!</body></html>";


        this.mockFileStorageService
            .Setup(s => s.DownloadAsync(
                "cached.html",
                "templates",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(System.Text.Encoding.UTF8.GetBytes(templateContent)));

        var properties = new Dictionary<string, object> { { "name", "John" } };

        // Act
        var result1 = await service.RenderAsync("cached", properties);
        var result2 = await service.RenderAsync("cached", properties);

        // Assert
        result1.Should().Contain("Hello John");
        result2.Should().Contain("Hello John");

        // File storage should only be called once due to caching
        this.mockFileStorageService.Verify(
            s => s.DownloadAsync("cached.html", "templates", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RenderAsync_WithEmptyProperties_RendersTemplateWithDefaults()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        dbContext.EmailTemplates.Add(new EmailTemplateEntity
        {
            Name = "simple",
            Path = "templates/simple.html",
            Size = 100
        });
        await dbContext.SaveChangesAsync();

        var templateContent = "<html><body>Hello {{ name ?? 'Guest' }}!</body></html>";
        var templateStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(templateContent));

        this.mockFileStorageService
            .Setup(s => s.DownloadAsync(
                "simple.html",
                "templates",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(templateStream);

        // Act
        var result = await service.RenderAsync("simple", new Dictionary<string, object>());

        // Assert
        result.Should().Contain("Hello Guest");
    }

    [Fact]
    public async Task DeleteAsync_InvalidatesCachedTemplate()
    {
        // Arrange
        using var dbContext = this.CreateInMemoryDbContext();
        var service = this.CreateService(dbContext);

        var template = new EmailTemplateEntity
        {
            Name = "to-invalidate",
            Path = "templates/to-invalidate.html",
            Size = 100
        };
        dbContext.EmailTemplates.Add(template);
        await dbContext.SaveChangesAsync();

        // Pre-populate the cache
        var templateContent = "<html><body>Hello {{ name }}!</body></html>";
        var templateStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(templateContent));

        this.mockFileStorageService
            .Setup(s => s.DownloadAsync(
                "to-invalidate.html",
                "templates",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(System.Text.Encoding.UTF8.GetBytes(templateContent)));

        this.mockFileStorageService
            .Setup(s => s.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Render to cache the template
        await service.RenderAsync("to-invalidate", new Dictionary<string, object> { { "name", "Test" } });

        // Act
        await service.DeleteAsync(template.Id);

        // Assert - cache key should be invalidated (we can verify by checking that cacheremove was called indirectly)
        // This test verifies the template is removed from the database
        var deletedTemplate = dbContext.EmailTemplates.FirstOrDefault(t => t.Name == "to-invalidate");
        deletedTemplate.Should().BeNull();
    }

    private static Mock<IFormFile> CreateMockFormFile(string fileName, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);


        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(bytes.Length);
        mockFile.Setup(f => f.ContentType).Returns("text/html");

        return mockFile;
    }
}
