using account_service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace account_service.Features.Accounts;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<Account> CreateAsync(Account account);
    Task<Account?> UpdateAsync(int id, Account account);
    Task<bool> DeleteAsync(int id);
}

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task<IEnumerable<Account>> GetAllAsync()
        => await context.Accounts.ToListAsync();

    public async Task<Account?> GetByIdAsync(int id)
        => await context.Accounts.FindAsync(id);

    public async Task<Account> CreateAsync(Account account)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    public async Task<Account?> UpdateAsync(int id, Account account)
    {
        var existing = await context.Accounts.FindAsync(id);
        if (existing is null) return null;

        existing.Name = account.Name;
        existing.Balance = account.Balance;
        existing.Available = account.Available;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await context.Accounts.FindAsync(id);
        if (existing is null) return false;

        context.Accounts.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
