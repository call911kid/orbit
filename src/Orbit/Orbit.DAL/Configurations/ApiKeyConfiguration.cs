using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbit.DAL.Models;

namespace Orbit.DAL.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.KeyPrefix)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(k => k.KeyHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(k => k.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(k => k.ExpiresAt)
            .IsRequired();

        builder.HasIndex(k => k.KeyHash)
            .IsUnique();

        builder.HasQueryFilter(k => !k.IsDeleted);
    }
}
