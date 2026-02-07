using Microservice.Email.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Email.Infrastructure.Persistence.EntityConfigurations;

/// <summary>
/// Configures the EmailTemplateEntity for Entity Framework Core.
/// </summary>
public sealed class EmailTemplateEntityConfiguration : IEntityTypeConfiguration<EmailTemplateEntity>
{
    /// <summary>
    /// Configures the EmailTemplateEntity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<EmailTemplateEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Path)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.Property(e => e.Size)
            .IsRequired();
    }
}
