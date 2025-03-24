using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;

namespace PegueFacilPay.Infrastructure.Data
{
    public class DatabaseMigrator
    {
        private readonly DatabaseConnection _db;
        private readonly ILogger<DatabaseMigrator> _logger;

        public DatabaseMigrator(DatabaseConnection db, ILogger<DatabaseMigrator> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task MigrateAsync()
        {
            try
            {
                _logger.LogInformation("Starting database migration...");

                await CreateTablesAsync();
                await SeedDataAsync();

                _logger.LogInformation("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database migration.");
                throw;
            }
        }

        private async Task CreateTablesAsync()
        {
            try
            {
                _logger.LogInformation("Creating database tables...");

                var createTablesSql = await File.ReadAllTextAsync("Data/Scripts/CreateTables.sql");
                using var connection = _db.CreateConnection();
                await connection.ExecuteAsync(createTablesSql);

                _logger.LogInformation("Database tables created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database tables.");
                throw;
            }
        }

        public async Task DropTablesAsync()
        {
            try
            {
                _logger.LogWarning("Dropping all database tables...");

                var dropTablesSql = await File.ReadAllTextAsync("Data/Scripts/DropTables.sql");
                using var connection = _db.CreateConnection();
                await connection.ExecuteAsync(dropTablesSql);

                _logger.LogInformation("Database tables dropped successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dropping database tables.");
                throw;
            }
        }

        public async Task SeedDataAsync()
        {
            try
            {
                _logger.LogInformation("Seeding test data...");

                var seedDataSql = await File.ReadAllTextAsync("Data/Scripts/SeedData.sql");
                using var connection = _db.CreateConnection();
                await connection.ExecuteAsync(seedDataSql);

                _logger.LogInformation("Test data seeded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding test data.");
                throw;
            }
        }

        public async Task ResetDatabaseAsync()
        {
            try
            {
                _logger.LogWarning("Resetting database...");

                await DropTablesAsync();
                await CreateTablesAsync();
                await SeedDataAsync();

                _logger.LogInformation("Database reset completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting database.");
                throw;
            }
        }

        public async Task<bool> CheckDatabaseExistsAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string sql = @"
                    SELECT COUNT(1) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME IN ('Users', 'Wallets', 'Transactions')";
                
                var tableCount = await connection.ExecuteScalarAsync<int>(sql);
                return tableCount == 3;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database existence.");
                return false;
            }
        }
    }
} 