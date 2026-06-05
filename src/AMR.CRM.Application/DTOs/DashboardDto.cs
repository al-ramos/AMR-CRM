namespace AMR.CRM.Application.DTOs;

public record DashboardKpisDto(
    int     TotalLeads,
    int     LeadsAtivos,
    int     OportunidadesAbertas,
    decimal PipelinePonderado,
    decimal TaxaConversao);

public record FunilLeadItemDto(string Status, int Count, decimal Valor);
public record FunilEtapaItemDto(string Etapa, int Count, decimal Valor);
public record FunilDto(List<FunilLeadItemDto> Leads, List<FunilEtapaItemDto> Oportunidades);

public record TopOportunidadeDto(
    Guid    Id,
    string  Titulo,
    string? Lead,
    string? Contato,
    decimal Valor,
    int     Probabilidade,
    decimal ValorPonderado,
    string  Etapa,
    string  Status);

public record EvolucaoItemDto(string Mes, int Abertas, int Fechadas);
