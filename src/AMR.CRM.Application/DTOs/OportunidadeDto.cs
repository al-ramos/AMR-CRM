using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.DTOs;

public record OportunidadeDto(
    Guid               Id,
    Guid               ContatoId,
    string             ContatoNome,
    string             Titulo,
    decimal            Valor,
    StatusOportunidade Status,
    string             StatusNome,
    string?            Descricao,
    DateTime?          PrevisaoFechamento,
    DateTime           CriadoEm
);
