using System.ComponentModel.DataAnnotations;

namespace account_service.Features.Accounts.Dtos;

public record CreateAccountDto(
    [Required][MaxLength(100)] string Name,
    decimal Balance,
    decimal Available
);
