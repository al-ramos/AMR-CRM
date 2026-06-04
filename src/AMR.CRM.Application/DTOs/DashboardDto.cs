namespace AMR.CRM.Application.DTOs;

public record DashboardDto(
    int     TotalContatos,
    int     TotalOportunidades,
    decimal ValorPipeline,
    decimal TaxaConversao,
    decimal TicketMedio,
    IReadOnlyList<KanbanColunaDto>     KanbanColunas,
    IReadOnlyList<OportunidadesMesDto> OportunidadesPorMes
);

public record KanbanColunaDto(
    int     Status,
    string  StatusNome,
    int     Total,
    decimal ValorTotal,
    IReadOnlyList<KanbanCardDto> Cards
);

public record KanbanCardDto(
    Guid    Id,
    string  Titulo,
    string  ContatoNome,
    decimal Valor
);

public record OportunidadesMesDto(
    string  Mes,
    int     Total,
    decimal Valor
);
