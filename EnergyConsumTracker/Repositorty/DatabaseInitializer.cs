using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnergyConsumTracker.Repositorty
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database created successfully");

                // Check if data already exists
                if (!context.Consumers.Any())
                {
                    logger.LogInformation("Seeding initial data...");
                    // The seed data will be automatically applied by Entity Framework
                    // since it's configured in the OnModelCreating method
                }
                else
                {
                    logger.LogInformation("Database already contains data, skipping seed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        public static void InitializeDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Ensure database is created
                context.Database.EnsureCreated();
                logger.LogInformation("Database created successfully");

                // Check if data already exists
                if (!context.Consumers.Any())
                {
                    logger.LogInformation("Seeding initial data...");
                    // The seed data will be automatically applied by Entity Framework
                    // since it's configured in the OnModelCreating method
                }
                else
                {
                    logger.LogInformation("Database already contains data, skipping seed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }
    }
} 