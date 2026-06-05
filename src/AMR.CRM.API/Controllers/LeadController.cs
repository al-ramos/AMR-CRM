using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.CRM.Application.Leads.Commands;
using AMR.CRM.Application.Leads.Queries;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todos os leads (com filtro opcional por status).</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] StatusLead? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarLeadsQuery(status), ct);
        return Ok(result);
    }

    /// <summary>Retorna um lead pelo id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterLeadQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Cria um novo lead.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarLeadRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CriarLeadCommand(req.Nome, req.Email, req.Origem,
                req.Telefone, req.Empresa, req.ValorEstimado, req.Notas), ct);

        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor);
    }

    /// <summary>Atualiza dados de um lead.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id,
        [FromBody] AtualizarLeadRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AtualizarLeadCommand(id, req.Nome, req.Email, req.Origem,
                req.Telefone, req.Empresa, req.ValorEstimado, req.Notas), ct);

        if (!result.Sucesso) return NotFound(new { erro = result.Erro });
        return NoContent();
    }

    /// <summary>Avança o status do lead no pipeline.</summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AvancarStatus(Guid id,
        [FromBody] AvancarStatusRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new AvancarStatusLeadCommand(id, req.NovoStatus), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    /// <summary>Exclui um lead (somente sem oportunidades vinculadas).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcluirLeadCommand(id), ct);
        if (!result.Sucesso) return BadRequest(new { erro = result.Erro });
        return NoContent();
    }

    /// <summary>Importa clientes selecionados do AMR-Core como novos Leads.</summary>
    [HttpPost("importar-de-core")]
    public async Task<IActionResult> ImportarDeCore(
        [FromBody] ImportarDeCoreRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ImportarLeadsFromCoreCommand(req.ClienteIds), ct);
        if (!result.Sucesso)
            return StatusCode(503, new { erro = result.Erro });
        return Ok(new { importados = result.Valor!.Importados, ignorados = result.Valor.Ignorados });
    }
}

public record CriarLeadRequest(
    string     Nome,
    string     Email,
    OrigemLead Origem,
    string?    Telefone      = null,
    string?    Empresa       = null,
    decimal    ValorEstimado = 0m,
    string?    Notas         = null
);

public record AtualizarLeadRequest(
    string     Nome,
    string     Email,
    OrigemLead Origem,
    string?    Telefone      = null,
    string?    Empresa       = null,
    decimal    ValorEstimado = 0m,
    string?    Notas         = null
);

public record AvancarStatusRequest(StatusLead NovoStatus);

public record ImportarDeCoreRequest(List<int> ClienteIds);
