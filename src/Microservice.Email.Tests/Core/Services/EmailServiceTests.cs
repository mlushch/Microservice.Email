using Microservice.Email.Core.Exceptions;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Core.Services;
using Microservice.Email.Core.Validation;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Domain.Enums;
using Microservice.Email.Domain.Models;
using Microservice.Email.Infrastructure.Persistence;

using Microsoft.Extensions.Logging;

namespace Microservice.Email.Tests.Core.Services;

/// <summary>
/// Unit tests for the EmailService class.
/// </summary>
public sealed class EmailServiceTests : TestBase
{
    private readonly Mock<ISmtpService> mockSmtpService;
    private readonly Mock<ITemplateService> mockTemplateService;
    private readonly Mock<IValidator<SendEmailRequest>> mockEmailValidator;
    private readonly Mock<IValidator<SendTemplatedEmailRequest>> mockTemplatedEmailValidator;
    private readonly Mock<ILogger<EmailService>> mockLogger;

    public EmailServiceTests()
    {
        this.mockSmtpService = new Mock<ISmtpService>();
        this.mockTemplateService = new Mock<ITemplateService>();
        this.mockEmailValidator = new Mock<IValidator<SendEmailRequest>>();
        this.mockTemplatedEmailValidator = new Mock<IValidator<SendTemplatedEmailRequest>>();
        this.mockLogger = new Mock<ILogger<EmailService>>();
    }

    private EmailService CreateService(EmailDbContext dbContext)
    {
        return new EmailService(
            dbContext,
            mockSmtpService.Object,
            mockTemplateService.Object,
            mockEmailValidator.Object,
            mockTemplatedEmailValidator.Object,
            mockLogger.Object);
    }

    [Fact]
    public async Task SendAsync_WithValidRequest_ReturnsEmailResponseWithSentStatus()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = new SendEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com", Name = "Test Sender" },
                Recipients = new[] { "recipient@example.com" },
                Subject = "Test Subject",
                Body = "Test Body"
            }
        };

        mockEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.EmailStatus.Should().Be(EmailStatus.Sent);
        result.Sender.Email.Should().Be("sender@example.com");
        result.Recipients.Should().HaveCount(1);
        result.Recipients.First().Email.Should().Be("recipient@example.com");

        mockSmtpService.Verify(
            s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                "Test Subject",
                "Test Body",
                false,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithValidationFailure_ThrowsValidationException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = new SendEmailRequest
            {
                Sender = new Sender { Email = "" },
                Recipients = Array.Empty<string>(),
                Subject = "",
                Body = ""
            }
        };

        mockEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendEmailRequest>()))
            .Returns(ValidationResult.Failure("Recipients", "Recipients are required"));

        // Act
        var act = async () => await service.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(e => e.Errors.Any(err => err.PropertyName == "Recipients"));
    }

    [Fact]
    public async Task SendAsync_WhenSmtpFails_ThrowsEmailSendException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = new SendEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = new[] { "recipient@example.com" },
                Subject = "Test Subject",
                Body = "Test Body"
            }
        };

        mockEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("SMTP connection failed"));

        // Act
        var act = async () => await service.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<EmailSendException>()
            .WithMessage("Failed to send email.");
    }

    [Fact]
    public async Task SendAsync_WithMultipleRecipients_SendsToAllRecipients()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var recipients = new[] { "recipient1@example.com", "recipient2@example.com", "recipient3@example.com" };
        var request = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = new SendEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = recipients,
                Subject = "Test Subject",
                Body = "Test Body"
            }
        };

        mockEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.SendAsync(request);

        // Assert
        result.Recipients.Should().HaveCount(3);
        mockSmtpService.Verify(
            s => s.SendAsync(
                It.IsAny<Sender>(),
                It.Is<string[]>(r => r.Length == 3),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithAttachments_PassesAttachmentsToSmtpService()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var attachments = new[]
        {
            new Attachment { FileName = "test.txt", ContentBase64 = Convert.ToBase64String("test content"u8.ToArray()), ContentType = "text/plain" }
        };

        var request = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = new SendEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = new[] { "recipient@example.com" },
                Subject = "Test Subject",
                Body = "Test Body"
            },
            Attachments = attachments
        };

        mockEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.SendAsync(request);

        // Assert
        mockSmtpService.Verify(
            s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.Is<Attachment[]?>(a => a != null && a.Length == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_PersistsEmailToDatabase()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendEmailRequest>
        {
            Email = new SendEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com", Name = "Sender Name" },
                Recipients = new[] { "recipient@example.com" },
                Subject = "Test Subject",
                Body = "Test Body"
            }
        };

        mockEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.SendAsync(request);

        // Assert
        var savedEmail = dbContext.Emails.FirstOrDefault(e => e.Id == result.Id);
        savedEmail.Should().NotBeNull();
        savedEmail!.Subject.Should().Be("Test Subject");
        savedEmail.Body.Should().Be("Test Body");
        savedEmail.EmailStatus.Should().Be(EmailStatus.Sent);
    }

    [Fact]
    public async Task SendTemplatedAsync_WithValidRequest_RendersTemplateAndSends()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendTemplatedEmailRequest>
        {
            Email = new SendTemplatedEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = new[] { "recipient@example.com" },
                TemplateName = "welcome",
                TemplateProperties = new Dictionary<string, object>
                {
                    { "Subject", "Welcome Email" },
                    { "UserName", "John Doe" }
                }
            }
        };

        mockTemplatedEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendTemplatedEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockTemplateService
            .Setup(t => t.RenderAsync(
                "welcome",
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("<html><body>Welcome John Doe!</body></html>");

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.SendTemplatedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.EmailStatus.Should().Be(EmailStatus.Sent);

        mockTemplateService.Verify(
            t => t.RenderAsync("welcome", It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()),
            Times.Once);

        mockSmtpService.Verify(
            s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                "Welcome Email",
                "<html><body>Welcome John Doe!</body></html>",
                true, // isHtml should be true for templated emails
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendTemplatedAsync_WithValidationFailure_ThrowsValidationException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendTemplatedEmailRequest>
        {
            Email = new SendTemplatedEmailRequest
            {
                Sender = new Sender { Email = "" },
                Recipients = Array.Empty<string>(),
                TemplateName = "",
                TemplateProperties = new Dictionary<string, object>()
            }
        };

        mockTemplatedEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendTemplatedEmailRequest>()))
            .Returns(ValidationResult.Failure("TemplateName", "Template name is required"));

        // Act
        var act = async () => await service.SendTemplatedAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(e => e.Errors.Any(err => err.PropertyName == "TemplateName"));
    }

    [Fact]
    public async Task SendTemplatedAsync_WhenTemplateNotFound_ThrowsException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendTemplatedEmailRequest>
        {
            Email = new SendTemplatedEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = new[] { "recipient@example.com" },
                TemplateName = "nonexistent",
                TemplateProperties = new Dictionary<string, object>()
            }
        };

        mockTemplatedEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendTemplatedEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockTemplateService
            .Setup(t => t.RenderAsync(
                "nonexistent",
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TemplateNotFoundException("nonexistent"));

        // Act
        var act = async () => await service.SendTemplatedAsync(request);

        // Assert
        await act.Should().ThrowAsync<TemplateNotFoundException>();
    }

    [Fact]
    public async Task SendTemplatedAsync_WhenSmtpFails_SetsEmailStatusToFailed()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendTemplatedEmailRequest>
        {
            Email = new SendTemplatedEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = new[] { "recipient@example.com" },
                TemplateName = "welcome",
                TemplateProperties = new Dictionary<string, object>
                {
                    { "UserName", "John" }
                }
            }
        };

        mockTemplatedEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendTemplatedEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockTemplateService
            .Setup(t => t.RenderAsync(
                "welcome",
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("<html><body>Welcome!</body></html>");

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("SMTP failure"));

        // Act
        var act = async () => await service.SendTemplatedAsync(request);

        // Assert
        await act.Should().ThrowAsync<EmailSendException>();

        var savedEmail = dbContext.Emails.FirstOrDefault();
        savedEmail.Should().NotBeNull();
        savedEmail!.EmailStatus.Should().Be(EmailStatus.Failed);
    }

    [Fact]
    public async Task SendTemplatedAsync_UsesTemplateNameAsSubject_WhenNoSubjectProperty()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = CreateService(dbContext);

        var request = new AttachmentsWrapper<SendTemplatedEmailRequest>
        {
            Email = new SendTemplatedEmailRequest
            {
                Sender = new Sender { Email = "sender@example.com" },
                Recipients = new[] { "recipient@example.com" },
                TemplateName = "welcome",
                TemplateProperties = new Dictionary<string, object>
                {
                    { "UserName", "John" }
                }
            }
        };

        mockTemplatedEmailValidator
            .Setup(v => v.Validate(It.IsAny<SendTemplatedEmailRequest>()))
            .Returns(ValidationResult.Success());

        mockTemplateService
            .Setup(t => t.RenderAsync(
                "welcome",
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("<html><body>Welcome!</body></html>");

        mockSmtpService
            .Setup(s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.SendTemplatedAsync(request);

        // Assert
        mockSmtpService.Verify(
            s => s.SendAsync(
                It.IsAny<Sender>(),
                It.IsAny<string[]>(),
                "welcome", // Template name is used as subject
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Attachment[]?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
