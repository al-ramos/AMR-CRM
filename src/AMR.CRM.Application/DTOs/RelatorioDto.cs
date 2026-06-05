namespace AMR.CRM.Application.DTOs;

// Funil de conversão
public record FunilConversaoItemDto(
    string  Etapa,
    int     Total,
    decimal Valor,
    decimal TaxaConversao);

public record FunilConversaoDto(
    List<FunilConversaoItemDto> Itens,
    decimal TaxaConversaoGeral,
    int     PeriodoDias);

// Forecast mensal
public record ForecastOportunidadeDto(
    Guid     Id,
    string   Titulo,
    decimal  Valor,
    int      Probabilidade,
    decimal  ValorPonderado,
    DateTime PrevisaoFechamento,
    string   Status,
    string   Etapa);

public record ForecastDto(
    string   Mes,
    decimal  ReceitaEsperada,
    decimal  ReceitaConfirmada,
    int      TotalOportunidades,
    List<ForecastOportunidadeDto> Oportunidades);

// Análise de leads
public record LeadsPorOrigemItemDto(string Origem, int Total, decimal ValorTotal);
public record LeadsPorStatusItemDto(string Status, int Total, decimal ValorTotal);

public record AnaliseLeadsDto(
    List<LeadsPorOrigemItemDto>  PorOrigem,
    List<LeadsPorStatusItemDto>  PorStatus,
    int                          TotalLeads,
    decimal                      ValorTotalPipeline,
    int                          PeriodoDias);
