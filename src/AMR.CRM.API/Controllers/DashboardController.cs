using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Dashboard.Queries;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>KPIs principais do CRM.</summary>
    [HttpGet]
    public async Task<IActionResult> Kpis(CancellationToken ct)
        => Ok(await mediator.Send(new GetDashboardKpisQuery(), ct));

    /// <summary>Contagem de leads por status e oportunidades por etapa.</summary>
    [HttpGet("funil")]
    public async Task<IActionResult> Funil(CancellationToken ct)
        => Ok(await mediator.Send(new GetFunilQuery(), ct));

    /// <summary>Top 5 oportunidades por valor (excluindo canceladas).</summary>
    [HttpGet("top-oportunidades")]
    public async Task<IActionResult> TopOportunidades(CancellationToken ct)
        => Ok(await mediator.Send(new GetTopOportunidadesQuery(), ct));

    /// <summary>Evolução de oportunidades abertas vs fechadas por mês.</summary>
    [HttpGet("evolucao")]
    public async Task<IActionResult> Evolucao([FromQuery] int periodo = 30, CancellationToken ct = default)
        => Ok(await mediator.Send(new GetEvolucaoQuery(periodo), ct));
}
