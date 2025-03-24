using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PegueFacilPay.Domain.Interfaces;
using PegueFacilPay.Infrastructure.Data;
using PegueFacilPay.Infrastructure.Repositories;
using PegueFacilPay.Infrastructure.Services;

namespace PegueFacilPay.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddSingleton<DatabaseConnection>();
            services.AddScoped<DatabaseMigrator>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            // Services
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
} 