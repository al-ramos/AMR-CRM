using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.DTOs;

public record OportunidadeDto(
    Guid               Id,
    Guid?              ContatoId,
    string?            ContatoNome,
    Guid?              LeadId,
    string?            LeadNome,
    string             Titulo,
    decimal            Valor,
    int                Probabilidade,
    StatusOportunidade Status,
    string             StatusNome,
    string?            Descricao,
    DateTime?          PrevisaoFechamento,
    DateTime           CriadoEm
);
