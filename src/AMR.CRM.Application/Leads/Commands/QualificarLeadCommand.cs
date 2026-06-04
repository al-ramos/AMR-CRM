namespace AMR.CRM.Application.Leads.Commands;

public record QualificarLeadCommand(Guid Id) : IRequest<Result>;

public class QualificarLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<QualificarLeadCommand, Result>
{
    public async Task<Result> Handle(QualificarLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");
        lead.Qualificar();
        repo.Atualizar(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
