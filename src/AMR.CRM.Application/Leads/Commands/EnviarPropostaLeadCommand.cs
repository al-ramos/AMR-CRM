namespace AMR.CRM.Application.Leads.Commands;

public record EnviarPropostaLeadCommand(Guid Id) : IRequest<Result>;

public class EnviarPropostaLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<EnviarPropostaLeadCommand, Result>
{
    public async Task<Result> Handle(EnviarPropostaLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");
        lead.EnviarProposta();
        repo.Atualizar(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
