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
            new CriarOportunidadeCommand(req.ContatoId, req.Titulo, req.Valor,
                req.Descricao, req.PrevisaoFechamento, req.LeadId, req.Probabilidade), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return Created(string.Empty, result.Valor);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id,
        [FromBody] AtualizarOportunidadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AtualizarOportunidadeCommand(id, req.Titulo, req.Valor,
                req.Probabilidade, req.Descricao, req.PrevisaoFechamento), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return Ok(result.Valor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcluirOportunidadeCommand(id), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    [HttpPatch("{id:guid}/iniciar")]
    public async Task<IActionResult> Iniciar(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new AvancarOportunidadeCommand(id, StatusOportunidade.EmAndamento), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    [HttpPatch("{id:guid}/ganhar")]
    public async Task<IActionResult> Ganhar(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new AvancarOportunidadeCommand(id, StatusOportunidade.Ganha), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    [HttpPatch("{id:guid}/perder")]
    public async Task<IActionResult> Perder(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new AvancarOportunidadeCommand(id, StatusOportunidade.Perdida), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new AvancarOportunidadeCommand(id, StatusOportunidade.Cancelada), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }
}

public record CriarOportunidadeRequest(
    string    Titulo,
    decimal   Valor,
    Guid?     ContatoId          = null,
    Guid?     LeadId             = null,
    decimal   Probabilidade      = 0,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null
);

public record AtualizarOportunidadeRequest(
    string    Titulo,
    decimal   Valor,
    decimal   Probabilidade      = 0,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null
);
