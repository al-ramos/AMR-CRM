namespace AMR.CRM.Application.Leads.Commands;

public record GanharLeadCommand(Guid Id) : IRequest<Result>;

public class GanharLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<GanharLeadCommand, Result>
{
    public async Task<Result> Handle(GanharLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");
        lead.Ganhar();
        repo.Atualizar(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
