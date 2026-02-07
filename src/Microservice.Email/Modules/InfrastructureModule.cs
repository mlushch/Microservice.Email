using System.Net;
using System.Net.Mail;

using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;

using Microservice.Email.Core.Configuration;
using Microservice.Email.Core.Interfaces;
using Microservice.Email.Domain.Contracts;
using Microservice.Email.Infrastructure.Messaging;
using Microservice.Email.Infrastructure.Smtp;
using Microservice.Email.Infrastructure.Storage;

using Minio;

namespace Microservice.Email.Modules;

/// <summary>
/// Module for registering infrastructure services (MinIO, SMTP, RabbitMQ).
/// </summary>
public sealed class InfrastructureModule : IModule
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // MinIO configuration
        services.Configure<MinioSettings>(configuration.GetSection(MinioSettings.SectionName));

        var minioSettings = configuration.GetSection(MinioSettings.SectionName).Get<MinioSettings>();
        if (minioSettings is not null)
        {
            services.AddSingleton<IMinioClient>(_ =>
            {
                var client = new MinioClient()
                    .WithEndpoint(minioSettings.Endpoint)
                    .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey);

                if (minioSettings.UseSSL)
                {
                    client = client.WithSSL();
                }

                return client.Build();
            });
        }

        services.AddScoped<IFileStorageService, FileStorageService>();

        // SMTP configuration
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));

        var smtpSettings = configuration.GetSection(SmtpSettings.SectionName).Get<SmtpSettings>();
        if (smtpSettings is not null)
        {
            var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
            {
                EnableSsl = smtpSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrEmpty(smtpSettings.Username) && !string.IsNullOrEmpty(smtpSettings.Password))
            {
                smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
            }

            services.AddFluentEmail(smtpSettings.DefaultSenderEmail ?? "noreply@example.com")
                .AddSmtpSender(smtpClient);
        }
        else
        {
            // Default configuration when SMTP settings are not provided
            services.AddFluentEmail("noreply@example.com")
                .AddSmtpSender("localhost", 25);
        }

        services.AddScoped<ISmtpService, SmtpService>();

        // RabbitMQ configuration
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

        services.AddScoped<IMessageHandler<AttachmentsWrapper<SendEmailRequest>>, SendEmailMessageHandler>();
        services.AddScoped<IMessageHandler<AttachmentsWrapper<SendTemplatedEmailRequest>>, SendTemplatedEmailMessageHandler>();
        services.AddHostedService<RabbitMqConsumerService>();
    }
}
