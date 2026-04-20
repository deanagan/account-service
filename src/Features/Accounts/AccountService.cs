using account_service.Features.Accounts.Dtos;

namespace account_service.Features.Accounts;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto?> GetByIdAsync(int id);
    Task<AccountDto> CreateAsync(CreateAccountDto dto);
    Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto dto);
    Task<bool> DeleteAsync(int id);
}

public class AccountService(IAccountRepository repository) : IAccountService
{
    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var accounts = await repository.GetAllAsync();
        return accounts.Select(ToDto);
    }

    public async Task<AccountDto?> GetByIdAsync(int id)
    {
        var account = await repository.GetByIdAsync(id);
        return account is null ? null : ToDto(account);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto)
    {
        var account = new Account
        {
            Name = dto.Name,
            Balance = dto.Balance,
            Available = dto.Available
        };

        var created = await repository.CreateAsync(account);
        return ToDto(created);
    }

    public async Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto dto)
    {
        var account = new Account
        {
            Name = dto.Name,
            Balance = dto.Balance,
            Available = dto.Available
        };

        var updated = await repository.UpdateAsync(id, account);
        return updated is null ? null : ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
        => await repository.DeleteAsync(id);

    private static AccountDto ToDto(Account account)
        => new(account.Id, account.Name, account.Balance, account.Available);
}
