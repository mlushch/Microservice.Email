using Microservice.Email.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Email.Infrastructure.Persistence.EntityConfigurations;

/// <summary>
/// Configures the EmailEntity for Entity Framework Core.
/// </summary>
public sealed class EmailEntityConfiguration : IEntityTypeConfiguration<EmailEntity>
{
    /// <summary>
    /// Configures the EmailEntity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<EmailEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Body)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SenderName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.SenderEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.SentDate)
            .IsRequired();

        builder.Property(e => e.EmailStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.HasMany(e => e.Recipients)
            .WithOne(r => r.Email_Navigation)
            .HasForeignKey(r => r.EmailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
