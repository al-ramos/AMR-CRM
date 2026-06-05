namespace AMR.CRM.Application.DTOs;

public record ClienteCoreDto(
    int     Id,
    string  Nome,
    string  Email,
    string? Telefone,
    string? Cnpj,
    string? Cpf
);
