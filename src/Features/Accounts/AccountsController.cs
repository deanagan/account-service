using account_service.Features.Accounts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace account_service.Features.Accounts;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    // GET: api/accounts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts()
    {
        var accounts = await accountService.GetAllAsync();
        return Ok(accounts);
    }

    // GET: api/accounts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetAccount(int id)
    {
        var account = await accountService.GetByIdAsync(id);
        return account is null ? NotFound() : Ok(account);
    }

    // POST: api/accounts
    [HttpPost]
    public async Task<ActionResult<AccountDto>> CreateAccount(CreateAccountDto dto)
    {
        var created = await accountService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAccount), new { id = created.Id }, created);
    }

    // PUT: api/accounts/5
    [HttpPut("{id}")]
    public async Task<ActionResult<AccountDto>> UpdateAccount(int id, UpdateAccountDto dto)
    {
        var updated = await accountService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE: api/accounts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var deleted = await accountService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
