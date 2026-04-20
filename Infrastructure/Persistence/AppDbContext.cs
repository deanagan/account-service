using account_service.Features.Accounts;
using Microsoft.EntityFrameworkCore;

namespace account_service.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Balance).HasPrecision(18, 2);
            entity.Property(a => a.Available).HasPrecision(18, 2);
        });
    }
}
