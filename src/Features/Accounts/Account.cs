namespace account_service.Features.Accounts;

public class Account
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Balance { get; set; }
    public decimal Available { get; set; }
}