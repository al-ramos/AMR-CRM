using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.DTOs;

public record ContatoDto(
    Guid           Id,
    string         Nome,
    string         Email,
    string?        Telefone,
    string?        Empresa,
    TipoContato    Tipo,
    string         TipoNome,
    StatusContato  Status,
    string         StatusNome,
    string?        Notas,
    DateTime       CriadoEm
);
