using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Leads.Commands;

public record AtualizarLeadCommand(
    Guid       Id,
    string     Nome,
    string     Email,
    OrigemLead Origem,
    string?    Telefone      = null,
    string?    Empresa       = null,
    decimal    ValorEstimado = 0m,
    string?    Notas         = null
) : IRequest<Result>;

public class AtualizarLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<AtualizarLeadCommand, Result>
{
    public async Task<Result> Handle(AtualizarLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");

        lead.Atualizar(req.Nome, req.Email, req.Origem,
            req.Telefone, req.Empresa, req.ValorEstimado, req.Notas);

        repo.Atualizar(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
