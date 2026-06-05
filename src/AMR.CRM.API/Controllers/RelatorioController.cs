using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Relatorios.Queries;
using AMR.CRM.Application.DTOs;
using System.Text;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/relatorios")]
public class RelatorioController(IMediator mediator) : ControllerBase
{
    /// <summary>Funil de conversão de leads por etapa no período.</summary>
    [HttpGet("funil")]
    public async Task<IActionResult> Funil([FromQuery] int periodo = 30, CancellationToken ct = default)
        => Ok(await mediator.Send(new GetFunilConversaoQuery(periodo), ct));

    /// <summary>Forecast de receita mensal com base em oportunidades.</summary>
    [HttpGet("forecast")]
    public async Task<IActionResult> Forecast([FromQuery] string? mes = null, CancellationToken ct = default)
    {
        mes ??= DateTime.UtcNow.ToString("yyyy-MM");
        return Ok(await mediator.Send(new GetForecastMensalQuery(mes), ct));
    }

    /// <summary>Análise de leads por Origem e por Status no período.</summary>
    [HttpGet("leads")]
    public async Task<IActionResult> Leads([FromQuery] int periodo = 30, CancellationToken ct = default)
        => Ok(await mediator.Send(new GetAnaliseLeadsQuery(periodo), ct));

    /// <summary>Export CSV do funil de conversão.</summary>
    [HttpGet("funil/export")]
    public async Task<IActionResult> ExportFunil([FromQuery] int periodo = 30, CancellationToken ct = default)
    {
        var data = await mediator.Send(new GetFunilConversaoQuery(periodo), ct);
        var csv  = BuildFunilCsv(data);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"funil-conversao-{periodo}d.csv");
    }

    /// <summary>Export CSV do forecast mensal.</summary>
    [HttpGet("forecast/export")]
    public async Task<IActionResult> ExportForecast([FromQuery] string? mes = null, CancellationToken ct = default)
    {
        mes ??= DateTime.UtcNow.ToString("yyyy-MM");
        var data = await mediator.Send(new GetForecastMensalQuery(mes), ct);
        var csv  = BuildForecastCsv(data);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"forecast-{mes}.csv");
    }

    /// <summary>Export CSV da análise de leads.</summary>
    [HttpGet("leads/export")]
    public async Task<IActionResult> ExportLeads([FromQuery] int periodo = 30, CancellationToken ct = default)
    {
        var data = await mediator.Send(new GetAnaliseLeadsQuery(periodo), ct);
        var csv  = BuildLeadsCsv(data);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"leads-{periodo}d.csv");
    }

    // ─── CSV builders ────────────────────────────────────────────────────────

    private static string BuildFunilCsv(FunilConversaoDto d)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Etapa;Total;Valor (R$);Taxa Conversão (%)");
        foreach (var i in d.Itens)
            sb.AppendLine($"{i.Etapa};{i.Total};{i.Valor:F2};{i.TaxaConversao:F1}");
        sb.AppendLine($"TOTAL GERAL;;;{d.TaxaConversaoGeral:F1}");
        return sb.ToString();
    }

    private static string BuildForecastCsv(ForecastDto d)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Mês: {d.Mes}");
        sb.AppendLine($"Receita Esperada (R$);{d.ReceitaEsperada:F2}");
        sb.AppendLine($"Receita Confirmada (R$);{d.ReceitaConfirmada:F2}");
        sb.AppendLine();
        sb.AppendLine("Título;Valor (R$);Probabilidade (%);Valor Ponderado (R$);Previsão;Status;Etapa");
        foreach (var o in d.Oportunidades)
            sb.AppendLine($"{Esc(o.Titulo)};{o.Valor:F2};{o.Probabilidade};{o.ValorPonderado:F2};{o.PrevisaoFechamento:dd/MM/yyyy};{o.Status};{o.Etapa}");
        return sb.ToString();
    }

    private static string BuildLeadsCsv(AnaliseLeadsDto d)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Período: últimos {d.PeriodoDias} dias | Total Leads: {d.TotalLeads} | Pipeline: R$ {d.ValorTotalPipeline:F2}");
        sb.AppendLine();
        sb.AppendLine("=== Por Origem ===");
        sb.AppendLine("Origem;Total;Valor Total (R$)");
        foreach (var o in d.PorOrigem)
            sb.AppendLine($"{o.Origem};{o.Total};{o.ValorTotal:F2}");
        sb.AppendLine();
        sb.AppendLine("=== Por Status ===");
        sb.AppendLine("Status;Total;Valor Total (R$)");
        foreach (var s in d.PorStatus)
            sb.AppendLine($"{s.Status};{s.Total};{s.ValorTotal:F2}");
        return sb.ToString();
    }

    private static string Esc(string v) => v.Contains(';') ? $"\"{v}\"" : v;
}
