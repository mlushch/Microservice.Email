using Microservice.Email.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Microservice.Email.Infrastructure.Persistence;

/// <summary>
/// Database context for the Email microservice using Entity Framework Core with PostgreSQL.
/// </summary>
public sealed class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for email entities.
    /// </summary>
    public DbSet<EmailEntity> Emails { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for recipient entities.
    /// </summary>
    public DbSet<RecipientEntity> Recipients { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for email template entities.
    /// </summary>
    public DbSet<EmailTemplateEntity> EmailTemplates { get; set; }

    /// <summary>
    /// Configures the entity mappings and model-building conventions for the EmailDbContext.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmailDbContext).Assembly);

    }
}
