using account_service.Features.Accounts;
using account_service.Features.Accounts.Dtos;
using NSubstitute;
using Xunit;

namespace account_service.Tests.Unit.Accounts;

public class AccountServiceTests
{
    private readonly IAccountRepository _repository;
    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        _repository = Substitute.For<IAccountRepository>();
        _sut = new AccountService(_repository);
    }

    // ── GetAllAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllAccountsAsDtos()
    {
        var accounts = new[]
        {
            new Account { Id = 1, Name = "Savings", Balance = 1000m, Available = 1000m },
            new Account { Id = 2, Name = "Checking", Balance = 500m, Available = 500m }
        };
        _repository.GetAllAsync().Returns(accounts);

        var result = await _sut.GetAllAsync();

        var dtos = result.ToList();
        Assert.Equal(2, dtos.Count);
        Assert.Equal("Savings", dtos[0].Name);
        Assert.Equal("Checking", dtos[1].Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoAccounts()
    {
        _repository.GetAllAsync().Returns([]);

        var result = await _sut.GetAllAsync();

        Assert.Empty(result);
    }

    // ── GetByIdAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenAccountExists()
    {
        var account = new Account { Id = 1, Name = "Savings", Balance = 1000m, Available = 900m };
        _repository.GetByIdAsync(1).Returns(account);

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Savings", result.Name);
        Assert.Equal(1000m, result.Balance);
        Assert.Equal(900m, result.Available);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenAccountDoesNotExist()
    {
        _repository.GetByIdAsync(99).Returns((Account?)null);

        var result = await _sut.GetByIdAsync(99);

        Assert.Null(result);
    }

    // ── CreateAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ReturnsCreatedDto()
    {
        var dto = new CreateAccountDto("Savings", 1000m, 1000m);
        var created = new Account { Id = 1, Name = "Savings", Balance = 1000m, Available = 1000m };
        _repository.CreateAsync(Arg.Any<Account>()).Returns(created);

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(1, result.Id);
        Assert.Equal("Savings", result.Name);
        Assert.Equal(1000m, result.Balance);
    }

    [Fact]
    public async Task CreateAsync_MapsAllFieldsFromDto()
    {
        var dto = new CreateAccountDto("Investment", 5000m, 4500m);
        Account? captured = null;
        _repository.CreateAsync(Arg.Do<Account>(a => captured = a))
                   .Returns(c => new Account { Id = 3, Name = c.Arg<Account>().Name, Balance = c.Arg<Account>().Balance, Available = c.Arg<Account>().Available });

        await _sut.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("Investment", captured!.Name);
        Assert.Equal(5000m, captured.Balance);
        Assert.Equal(4500m, captured.Available);
    }

    // ── UpdateAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedDto_WhenAccountExists()
    {
        var dto = new UpdateAccountDto("Updated Savings", 2000m, 1800m);
        var updated = new Account { Id = 1, Name = "Updated Savings", Balance = 2000m, Available = 1800m };
        _repository.UpdateAsync(1, Arg.Any<Account>()).Returns(updated);

        var result = await _sut.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated Savings", result!.Name);
        Assert.Equal(2000m, result.Balance);
        Assert.Equal(1800m, result.Available);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenAccountDoesNotExist()
    {
        var dto = new UpdateAccountDto("Ghost", 0m, 0m);
        _repository.UpdateAsync(99, Arg.Any<Account>()).Returns((Account?)null);

        var result = await _sut.UpdateAsync(99, dto);

        Assert.Null(result);
    }

    // ── DeleteAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenAccountExists()
    {
        _repository.DeleteAsync(1).Returns(true);

        var result = await _sut.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenAccountDoesNotExist()
    {
        _repository.DeleteAsync(99).Returns(false);

        var result = await _sut.DeleteAsync(99);

        Assert.False(result);
    }
}
