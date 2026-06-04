using AMR.CRM.Application.Leads.Commands;
using AMR.CRM.Application.Leads.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/lead")]
public class LeadController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarLeadsQuery(), ct);
        return Ok(result.Valor);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterLeadQuery(id), ct);
        return result.Sucesso ? Ok(result.Valor) : NotFound(new { erro = result.Erro });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarLeadCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor)
            : BadRequest(new { erro = result.Erro });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarLeadCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { Id = id }, ct);
        return result.Sucesso ? NoContent() : NotFound(new { erro = result.Erro });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcluirLeadCommand(id), ct);
        return result.Sucesso ? NoContent() : NotFound(new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/qualificar")]
    public async Task<IActionResult> Qualificar(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new QualificarLeadCommand(id), ct);
        return result.Sucesso ? NoContent() : UnprocessableEntity(new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/proposta")]
    public async Task<IActionResult> Proposta(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new EnviarPropostaLeadCommand(id), ct);
        return result.Sucesso ? NoContent() : UnprocessableEntity(new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/ganhar")]
    public async Task<IActionResult> Ganhar(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GanharLeadCommand(id), ct);
        return result.Sucesso ? NoContent() : UnprocessableEntity(new { erro = result.Erro });
    }

    [HttpPatch("{id:guid}/perder")]
    public async Task<IActionResult> Perder(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new PerderLeadCommand(id), ct);
        return result.Sucesso ? NoContent() : UnprocessableEntity(new { erro = result.Erro });
    }
}
