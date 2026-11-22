using Microsoft.EntityFrameworkCore;
using Orbit.DAL.Models;

namespace Orbit.DAL.Context;

public class OrbitDbContext : DbContext
{
    public OrbitDbContext(DbContextOptions<OrbitDbContext> options) : base(options)
    {
    }

    public DbSet<SearchProfile> SearchProfiles => Set<SearchProfile>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<ExcludedCompany> ExcludedCompanies => Set<ExcludedCompany>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically applies configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrbitDbContext).Assembly);
    }
}
