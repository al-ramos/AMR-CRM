using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.DTOs;

public record LeadDto(
    Guid          Id,
    string        Nome,
    string        Email,
    string?       Telefone,
    string?       Empresa,
    StatusLead    Status,
    string        StatusNome,
    OrigemLead    Origem,
    string        OrigemNome,
    decimal       ValorEstimado,
    string?       Notas,
    DateTime      CriadoEm,
    DateTime      AlteradoEm,
    int?          OrigemCoreClienteId
);
