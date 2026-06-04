using AMR.CRM.Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/crm/dashboard")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Obter(CancellationToken ct)
    {
        var result = await mediator.Send(new ObterDashboardQuery(), ct);
        return Ok(result.Valor);
    }
}
