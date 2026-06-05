using AMR.CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/integracao")]
public class IntegracaoController(ICoreApiClient coreApiClient) : ControllerBase
{
    /// <summary>Lista clientes do AMR-Core disponíveis para importação como Leads.</summary>
    [HttpGet("core/clientes")]
    public async Task<IActionResult> GetClientesCore(CancellationToken ct)
    {
        try
        {
            var clientes = await coreApiClient.GetClientesAsync(ct);
            return Ok(clientes);
        }
        catch (CoreApiUnavailableException ex)
        {
            return StatusCode(503, new { erro = ex.Message });
        }
    }
}
