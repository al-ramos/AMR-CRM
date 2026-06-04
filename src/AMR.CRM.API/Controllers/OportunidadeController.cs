using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Oportunidades.Commands;
using AMR.CRM.Application.Oportunidades.Queries;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OportunidadeController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarOportunidadesQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarOportunidadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CriarOportunidadeCommand(
                req.Titulo, req.Valor, req.Probabilidade ?? 50,
                req.ContatoId, req.LeadId,
                req.Descricao, req.PrevisaoFechamento), ct);

        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return Created(string.Empty, result.Valor);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AvancarStatus(Guid id,
        [FromBody] AvancarOportunidadeStatusRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new AvancarOportunidadeCommand(id, req.NovoStatus), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }
}

public record CriarOportunidadeRequest(
    string    Titulo,
    decimal   Valor,
    int?      Probabilidade      = null,
    Guid?     ContatoId          = null,
    Guid?     LeadId             = null,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null
);

public record AvancarOportunidadeStatusRequest(StatusOportunidade NovoStatus);
