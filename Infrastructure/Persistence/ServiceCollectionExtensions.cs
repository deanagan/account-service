using Microsoft.EntityFrameworkCore;

namespace account_service.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=accounts.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
