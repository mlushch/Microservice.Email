using Microservice.Email.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Email.Infrastructure.Persistence.EntityConfigurations;

/// <summary>
/// Configures the RecipientEntity for Entity Framework Core.
/// </summary>
public sealed class RecipientEntityConfiguration : IEntityTypeConfiguration<RecipientEntity>
{
    /// <summary>
    /// Configures the RecipientEntity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<RecipientEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.EmailId)
            .IsRequired();

        builder.HasOne(e => e.Email_Navigation)
            .WithMany(e => e.Recipients)
            .HasForeignKey(e => e.EmailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
