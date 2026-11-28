using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbit.DAL.Models;

namespace Orbit.DAL.Configurations;

public class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.HasKey(jp => jp.Id);

        builder.Property(jp => jp.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(jp => jp.Company)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(jp => jp.Location)
            .HasMaxLength(250);

        builder.Property(jp => jp.Url)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(1000);

        builder.Property(jp => jp.Source)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(jp => jp.DescriptionSnippet)
            .HasMaxLength(2000);

        builder.HasIndex(jp => new { jp.Source, jp.Url })
            .IsUnique();

        builder.HasIndex(jp => jp.CreatedAt);

        builder.HasQueryFilter(jp => !jp.IsDeleted);
    }
}
