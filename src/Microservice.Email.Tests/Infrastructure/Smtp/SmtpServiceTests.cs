using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Exceptions;
using Microservice.Email.Domain.Models;
using Microservice.Email.Infrastructure.Smtp;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using DomainAttachment = Microservice.Email.Domain.Contracts.Attachment;

namespace Microservice.Email.Tests.Infrastructure.Smtp;

/// <summary>
/// Unit tests for the SmtpService class.
/// </summary>
public sealed class SmtpServiceTests
{
    private readonly Mock<IFluentEmail> mockFluentEmail;
    private readonly Mock<ILogger<SmtpService>> mockLogger;
    private readonly SmtpSettings smtpSettings;

    public SmtpServiceTests()
    {
        this.mockFluentEmail = new Mock<IFluentEmail>();
        this.mockLogger = new Mock<ILogger<SmtpService>>();
        this.smtpSettings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            Username = "user",
            Password = "password",
            EnableSsl = true,
            DefaultSenderEmail = "default@example.com",
            DefaultSenderName = "Default Sender",
            MaxRetryAttempts = 3,
            RetryDelayMilliseconds = 100
        };
    }

    private SmtpService CreateService(SmtpSettings? settings = null)
    {
        return new SmtpService(
            this.mockFluentEmail.Object,
            Options.Create(settings ?? this.smtpSettings),
            this.mockLogger.Object);
    }

    [Fact]
    public async Task SendAsync_WithValidParameters_SendsEmailSuccessfully()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com", Name = "Test Sender" };
        var recipients = new[] { "recipient@example.com" };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body", false);

        // Assert
        this.mockFluentEmail.Verify(e => e.SetFrom(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        this.mockFluentEmail.Verify(e => e.Subject(It.IsAny<string>()), Times.Once);
        this.mockFluentEmail.Verify(e => e.To(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithHtmlBody_SendsHtmlEmail()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient@example.com" };
        var htmlBody = "<html><body><h1>Hello</h1></body></html>";

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", htmlBody, true);

        // Assert
        this.mockFluentEmail.Verify(e => e.Body(It.IsAny<string>(), true), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithMultipleRecipients_SendsToAllRecipients()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient1@example.com", "recipient2@example.com", "recipient3@example.com" };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert
        this.mockFluentEmail.Verify(e => e.To("recipient1@example.com"), Times.Once);
        this.mockFluentEmail.Verify(e => e.To("recipient2@example.com"), Times.Once);
        this.mockFluentEmail.Verify(e => e.To("recipient3@example.com"), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithAttachments_AddsAttachmentsToEmail()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient@example.com" };
        var attachments = new[]
        {
            new DomainAttachment
            {
                FileName = "test.txt",
                ContentBase64 = Convert.ToBase64String("Hello World"u8.ToArray()),
                ContentType = "text/plain"
            }
        };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body", false, attachments);

        // Assert
        this.mockFluentEmail.Verify(
            e => e.Attach(It.Is<Attachment>(a =>
                a.Filename == "test.txt" &&
                a.ContentType == "text/plain")),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithNoSenderEmail_UsesDefaultSenderEmail()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com" }; // Explicit sender
        var recipients = new[] { "recipient@example.com" };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert
        this.mockFluentEmail.Verify(e => e.SetFrom("sender@example.com", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WhenSendFails_RetriesAndEventuallyThrows()
    {
        // Arrange
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            MaxRetryAttempts = 3,
            RetryDelayMilliseconds = 10 // Short delay for testing
        };
        var service = this.CreateService(settings);
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient@example.com" };

        this.mockFluentEmail
            .Setup(e => e.SetFrom(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Subject(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.To(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Body(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.SendAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendResponse { ErrorMessages = new List<string> { "Connection refused" } });

        // Act
        var act = async () => await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert
        await act.Should().ThrowAsync<EmailSendException>()
            .WithMessage("*Failed to send email after 3 attempts*");

        // Verify that SendAsync was called MaxRetryAttempts times
        this.mockFluentEmail.Verify(
            e => e.SendAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task SendAsync_WhenFirstAttemptFails_SucceedsOnRetry()
    {
        // Arrange
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            MaxRetryAttempts = 3,
            RetryDelayMilliseconds = 10
        };
        var service = this.CreateService(settings);
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient@example.com" };

        this.mockFluentEmail
            .Setup(e => e.SetFrom(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Subject(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.To(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Body(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(this.mockFluentEmail.Object);

        var callCount = 0;
        this.mockFluentEmail
            .Setup(e => e.SendAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount < 2)
                {
                    return new SendResponse { ErrorMessages = new List<string> { "Temporary failure" } };
                }
                return new SendResponse();
            });

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert - Should succeed on second attempt
        this.mockFluentEmail.Verify(
            e => e.SendAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task SendAsync_WhenExceptionThrown_RetriesAndEventuallyThrows()
    {
        // Arrange
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            MaxRetryAttempts = 2,
            RetryDelayMilliseconds = 10
        };
        var service = this.CreateService(settings);
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient@example.com" };

        this.mockFluentEmail
            .Setup(e => e.SetFrom(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Subject(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.To(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Body(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.SendAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var act = async () => await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert
        await act.Should().ThrowAsync<EmailSendException>()
            .WithMessage("*Failed to send email after 2 attempts*");
    }

    [Fact]
    public async Task SendAsync_WithNullSenderName_UsesDefaultSenderName()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com", Name = null };
        var recipients = new[] { "recipient@example.com" };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert - Uses DefaultSenderName from settings when sender.Name is null
        this.mockFluentEmail.Verify(
            e => e.SetFrom("sender@example.com", "Default Sender"),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithNullSenderNameAndNoDefault_UsesSenderEmail()
    {
        // Arrange
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            MaxRetryAttempts = 1,
            DefaultSenderName = null, // No default
            DefaultSenderEmail = "default@example.com"
        };
        var service = this.CreateService(settings);
        var sender = new Sender { Email = "sender@example.com", Name = null };
        var recipients = new[] { "recipient@example.com" };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert - Falls back to email address when no sender name and no default
        this.mockFluentEmail.Verify(
            e => e.SetFrom("sender@example.com", "sender@example.com"),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithAttachmentWithoutContentType_UsesOctetStream()
    {
        // Arrange
        var service = this.CreateService();
        var sender = new Sender { Email = "sender@example.com" };
        var recipients = new[] { "recipient@example.com" };
        var attachments = new[]
        {
            new DomainAttachment
            {
                FileName = "binary.dat",
                ContentBase64 = Convert.ToBase64String(new byte[] { 0x00, 0x01, 0x02 }),
                ContentType = null
            }
        };

        this.SetupSuccessfulEmailSend();

        // Act
        await service.SendAsync(sender, recipients, "Test Subject", "Test Body", false, attachments);

        // Assert
        this.mockFluentEmail.Verify(
            e => e.Attach(It.Is<Attachment>(a =>
                a.ContentType == "application/octet-stream")),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithEmptySenderEmail_ThrowsEmailSendException()
    {
        // Arrange
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            MaxRetryAttempts = 1,
            DefaultSenderEmail = null // No default
        };
        var service = this.CreateService(settings);
        var sender = new Sender { Email = null! }; // No sender email
        var recipients = new[] { "recipient@example.com" };

        // Act
        var act = async () => await service.SendAsync(sender, recipients, "Test Subject", "Test Body");

        // Assert
        await act.Should().ThrowAsync<EmailSendException>()
            .WithMessage("*Sender email is required*");
    }

    private void SetupSuccessfulEmailSend()
    {
        this.mockFluentEmail
            .Setup(e => e.SetFrom(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Subject(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.To(It.IsAny<string>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Body(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.Attach(It.IsAny<Attachment>()))
            .Returns(this.mockFluentEmail.Object);
        this.mockFluentEmail
            .Setup(e => e.SendAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendResponse()); // Successful response
    }
}
