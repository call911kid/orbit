using Orbit.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Orbit.PL.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var dbContext = services.GetRequiredService<OrbitDbContext>();

            logger.LogInformation("Acquiring database migration lock...");
            await dbContext.Database.OpenConnectionAsync();

            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_getapplock @Resource = 'OrbitDbMigration', @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = 120000;");

                logger.LogInformation("Migration lock acquired. Applying pending database migrations...");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            finally
            {
                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "EXEC sp_releaseapplock @Resource = 'OrbitDbMigration', @LockOwner = 'Session';");
                }
                catch (Exception releaseEx)
                {
                    logger.LogWarning(releaseEx, "Failed to explicitly release migration lock; connection close will release it.");
                }

                await dbContext.Database.CloseConnectionAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to apply database migrations on application startup.");
            throw;
        }
    }
}
