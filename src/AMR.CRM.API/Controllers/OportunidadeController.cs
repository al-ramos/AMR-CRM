using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Oportunidades.Commands;
using AMR.CRM.Application.Oportunidades.Queries;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Application.DTOs;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterOportunidadeQuery(id), ct);
        if (!result.Sucesso) return NotFound(new { erro = result.Erro });
        return Ok(result.Valor);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarOportunidadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CriarOportunidadeCommand(
                req.Titulo, req.Valor, req.Probabilidade ?? 50,
                req.Etapa ?? EtapaOportunidade.Prospeccao,
                req.ContatoId, req.LeadId,
                req.Descricao, req.PrevisaoFechamento), ct);

        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return Created(string.Empty, result.Valor);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id,
        [FromBody] AtualizarOportunidadeRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AtualizarOportunidadeCommand(
                id, req.Titulo, req.Valor, req.Probabilidade ?? 50,
                req.Etapa ?? EtapaOportunidade.Prospeccao,
                req.Descricao, req.PrevisaoFechamento), ct);

        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcluirOportunidadeCommand(id), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
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
    string             Titulo,
    decimal            Valor,
    int?               Probabilidade      = null,
    EtapaOportunidade? Etapa              = null,
    Guid?              ContatoId          = null,
    Guid?              LeadId             = null,
    string?            Descricao          = null,
    DateTime?          PrevisaoFechamento = null
);

public record AtualizarOportunidadeRequest(
    string             Titulo,
    decimal            Valor,
    int?               Probabilidade      = null,
    EtapaOportunidade? Etapa              = null,
    string?            Descricao          = null,
    DateTime?          PrevisaoFechamento = null
);

public record AvancarOportunidadeStatusRequest(StatusOportunidade NovoStatus);
