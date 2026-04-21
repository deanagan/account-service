using account_service.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace account_service.IntegrationTests;

public class AccountsApiFactory : WebApplicationFactory<Program>
{
    // Fixed name so all scopes within one factory instance share the same in-memory database
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ALL DbContext-related registrations (including IDbContextOptionsConfiguration<T>
            // which carries the SQLite provider configuration added by AddPersistence)
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                         || d.ServiceType == typeof(AppDbContext)
                         || (d.ServiceType.IsGenericType &&
                             d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))
                         || d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration") == true)
                .ToList();
            foreach (var d in descriptors)
                services.Remove(d);

            // Replace with in-memory database, unique per test class instance
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });

        builder.UseEnvironment("Development");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    public void EnsureDbCreated()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}
