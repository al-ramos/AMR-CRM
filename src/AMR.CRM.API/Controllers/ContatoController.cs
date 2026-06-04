using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Contatos.Commands;
using AMR.CRM.Application.Contatos.Queries;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContatoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarContatosQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterContatoQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarContatoRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CriarContatoCommand(req.Nome, req.Email, req.Tipo, req.Telefone, req.Empresa, req.Notas), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarContatoRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AtualizarContatoCommand(id, req.Nome, req.Email, req.Tipo, req.Telefone, req.Empresa, req.Notas), ct);
        if (!result.Sucesso) return NotFound(new { erro = result.Erro });
        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    public async Task<IActionResult> Inativar(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new InativarContatoCommand(id), ct);
        if (!result.Sucesso) return NotFound(new { erro = result.Erro });
        return NoContent();
    }
}

public record CriarContatoRequest(
    string      Nome,
    string      Email,
    TipoContato Tipo,
    string?     Telefone = null,
    string?     Empresa  = null,
    string?     Notas    = null
);

public record AtualizarContatoRequest(
    string      Nome,
    string      Email,
    TipoContato Tipo,
    string?     Telefone = null,
    string?     Empresa  = null,
    string?     Notas    = null
);
