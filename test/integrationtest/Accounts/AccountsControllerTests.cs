using System.Net;
using System.Net.Http.Json;
using account_service.Features.Accounts.Dtos;
using account_service.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace account_service.IntegrationTests.Accounts;

public class AccountsControllerTests : IClassFixture<AccountsApiFactory>
{
    private readonly HttpClient _client;
    private readonly AccountsApiFactory _factory;

    public AccountsControllerTests(AccountsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        factory.EnsureDbCreated();
    }

    private async Task SeedAsync(params Action<AppDbContext>[] seedActions)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        foreach (var action in seedActions)
            action(db);
        await db.SaveChangesAsync();
    }

    // ── GET /api/accounts ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAccounts_ReturnsOk_WithEmptyList()
    {
        var response = await _client.GetAsync("/api/accounts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountDto>>();
        Assert.NotNull(accounts);
    }

    [Fact]
    public async Task GetAccounts_ReturnsAllSeededAccounts()
    {
        await SeedAsync(db =>
        {
            db.Accounts.Add(new account_service.Features.Accounts.Account { Name = "Savings", Balance = 1000m, Available = 1000m });
            db.Accounts.Add(new account_service.Features.Accounts.Account { Name = "Checking", Balance = 500m, Available = 500m });
        });

        var response = await _client.GetAsync("/api/accounts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountDto>>();
        Assert.NotNull(accounts);
        Assert.True(accounts!.Count >= 2);
    }

    // ── GET /api/accounts/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task GetAccount_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await _client.GetAsync("/api/accounts/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAccount_ReturnsOk_WhenExists()
    {
        var created = await _client.PostAsJsonAsync("/api/accounts",
            new CreateAccountDto("Integration Savings", 2000m, 2000m));
        var createdDto = await created.Content.ReadFromJsonAsync<AccountDto>();

        var response = await _client.GetAsync($"/api/accounts/{createdDto!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var account = await response.Content.ReadFromJsonAsync<AccountDto>();
        Assert.Equal("Integration Savings", account!.Name);
    }

    // ── POST /api/accounts ───────────────────────────────────────────────────

    [Fact]
    public async Task CreateAccount_ReturnsCreated_WithLocationHeader()
    {
        var dto = new CreateAccountDto("New Account", 500m, 500m);

        var response = await _client.PostAsJsonAsync("/api/accounts", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateAccount_ReturnsCreatedAccount()
    {
        var dto = new CreateAccountDto("Investment", 10000m, 9500m);

        var response = await _client.PostAsJsonAsync("/api/accounts", dto);
        var account = await response.Content.ReadFromJsonAsync<AccountDto>();

        Assert.NotNull(account);
        Assert.True(account!.Id > 0);
        Assert.Equal("Investment", account.Name);
        Assert.Equal(10000m, account.Balance);
        Assert.Equal(9500m, account.Available);
    }

    // ── PUT /api/accounts/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task UpdateAccount_ReturnsNotFound_WhenDoesNotExist()
    {
        var dto = new UpdateAccountDto("Ghost", 0m, 0m);

        var response = await _client.PutAsJsonAsync("/api/accounts/99999", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsUpdatedAccount()
    {
        var created = await _client.PostAsJsonAsync("/api/accounts",
            new CreateAccountDto("Before Update", 100m, 100m));
        var createdDto = await created.Content.ReadFromJsonAsync<AccountDto>();

        var updateDto = new UpdateAccountDto("After Update", 200m, 150m);
        var response = await _client.PutAsJsonAsync($"/api/accounts/{createdDto!.Id}", updateDto);
        var updated = await response.Content.ReadFromJsonAsync<AccountDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("After Update", updated!.Name);
        Assert.Equal(200m, updated.Balance);
        Assert.Equal(150m, updated.Available);
    }

    // ── DELETE /api/accounts/{id} ────────────────────────────────────────────

    [Fact]
    public async Task DeleteAccount_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/accounts/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsNoContent_WhenDeleted()
    {
        var created = await _client.PostAsJsonAsync("/api/accounts",
            new CreateAccountDto("To Delete", 100m, 100m));
        var createdDto = await created.Content.ReadFromJsonAsync<AccountDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/accounts/{createdDto!.Id}");
        var getResponse = await _client.GetAsync($"/api/accounts/{createdDto.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
