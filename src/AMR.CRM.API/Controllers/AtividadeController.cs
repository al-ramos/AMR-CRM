using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Atividades.Commands;
using AMR.CRM.Application.Atividades.Queries;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtividadeController(IMediator mediator) : ControllerBase
{
    [HttpGet("oportunidade/{oportunidadeId:guid}")]
    public async Task<IActionResult> Listar(Guid oportunidadeId, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarAtividadesQuery(oportunidadeId), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAtividadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CriarAtividadeCommand(req.OportunidadeId, req.Tipo, req.Descricao, req.DataHora), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return Created(string.Empty, result.Valor);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id,
        [FromBody] AtualizarAtividadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AtualizarAtividadeCommand(id, req.Tipo, req.Descricao, req.DataHora), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return Ok(result.Valor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcluirAtividadeCommand(id), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    [HttpPatch("{id:guid}/concluir")]
    public async Task<IActionResult> Concluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ConcluirAtividadeCommand(id), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }
}

public record CriarAtividadeRequest(
    Guid          OportunidadeId,
    TipoAtividade Tipo,
    string        Descricao,
    DateTime      DataHora
);

public record AtualizarAtividadeRequest(
    TipoAtividade Tipo,
    string        Descricao,
    DateTime      DataHora
);
