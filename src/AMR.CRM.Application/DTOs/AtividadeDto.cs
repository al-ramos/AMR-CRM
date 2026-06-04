using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.DTOs;

public record AtividadeDto(
    Guid          Id,
    Guid          OportunidadeId,
    TipoAtividade Tipo,
    string        TipoNome,
    string        Descricao,
    DateTime      DataHora,
    bool          Concluida,
    DateTime      CriadoEm
);
