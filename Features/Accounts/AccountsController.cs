using Microsoft.AspNetCore.Mvc;

namespace account_service.Features.Accounts
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        public class Account
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public decimal Balance { get; set; }
        }

        // GET: api/accounts
        [HttpGet]
        public ActionResult<Account[]> GetAccounts()
        {
            var accounts = new[]
            {
                new Account { Id = 1, Name = "Savings", Balance = 1000.50m },
                new Account { Id = 2, Name = "Checking", Balance = 500.25m },
                new Account { Id = 3, Name = "Investment", Balance = 12000.00m }
            };

            return Ok(accounts);
        }
    }
}