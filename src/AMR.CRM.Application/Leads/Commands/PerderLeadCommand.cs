namespace AMR.CRM.Application.Leads.Commands;

public record PerderLeadCommand(Guid Id) : IRequest<Result>;

public class PerderLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<PerderLeadCommand, Result>
{
    public async Task<Result> Handle(PerderLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");
        lead.Perder();
        repo.Atualizar(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
