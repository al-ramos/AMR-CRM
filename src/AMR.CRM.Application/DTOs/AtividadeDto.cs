using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.DTOs;

public record AtividadeDto(
    Guid            Id,
    string          Titulo,
    TipoAtividade   Tipo,
    string          TipoNome,
    DateTime        DataPrevista,
    StatusAtividade Status,
    string          StatusNome,
    string?         Observacao,
    Guid?           LeadId,
    string?         LeadNome,
    Guid?           OportunidadeId,
    string?         OportunidadeTitulo,
    bool            Vencida,
    DateTime        CriadoEm,
    DateTime        AlteradoEm
);
