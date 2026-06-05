using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Atividades.Commands;
using AMR.CRM.Application.Atividades.Queries;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtividadesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista atividades com filtros opcionais por status e tipo.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] StatusAtividade? status,
        [FromQuery] TipoAtividade?   tipo,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ListarAtividadesQuery(status, tipo), ct);
        return Ok(result);
    }

    /// <summary>Lista atividades vencidas (pendentes com data passada).</summary>
    [HttpGet("vencidas")]
    public async Task<IActionResult> Vencidas(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarAtividadesVencidasQuery(), ct);
        return Ok(result);
    }

    /// <summary>Retorna uma atividade pelo id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterAtividadeQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Cria uma nova atividade.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAtividadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CriarAtividadeCommand(req.Titulo, req.Tipo, req.DataPrevista,
                req.LeadId, req.OportunidadeId, req.Observacao), ct);

        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor);
    }

    /// <summary>Atualiza dados de uma atividade.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id,
        [FromBody] AtualizarAtividadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AtualizarAtividadeCommand(id, req.Titulo, req.Tipo, req.DataPrevista,
                req.Status, req.Observacao), ct);

        if (!result.Sucesso) return NotFound(new { erro = result.Erro });
        return NoContent();
    }

    /// <summary>Exclui uma atividade.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcluirAtividadeCommand(id), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }
}

public record CriarAtividadeRequest(
    string        Titulo,
    TipoAtividade Tipo,
    DateTime      DataPrevista,
    Guid?         LeadId         = null,
    Guid?         OportunidadeId = null,
    string?       Observacao     = null
);

public record AtualizarAtividadeRequest(
    string          Titulo,
    TipoAtividade   Tipo,
    DateTime        DataPrevista,
    StatusAtividade Status,
    string?         Observacao = null
);
