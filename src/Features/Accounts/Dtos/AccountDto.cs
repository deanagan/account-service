namespace account_service.Features.Accounts.Dtos;

public record AccountDto(int Id, string Name, decimal Balance, decimal Available);
