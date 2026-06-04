using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Leads.Commands;

public record AvancarStatusLeadCommand(Guid Id, StatusLead NovoStatus) : IRequest<Result>;

public class AvancarStatusLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<AvancarStatusLeadCommand, Result>
{
    public async Task<Result> Handle(AvancarStatusLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");

        lead.AvancarStatus(req.NovoStatus);
        repo.Atualizar(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
