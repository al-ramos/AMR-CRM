using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Application.Leads.Commands;

public record ExcluirLeadCommand(Guid Id) : IRequest<Result>;

public class ExcluirLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<ExcluirLeadCommand, Result>
{
    public async Task<Result> Handle(ExcluirLeadCommand req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha("Lead não encontrado.");

        if (lead.Oportunidades.Count > 0)
            return Result.Falha("Lead possui oportunidades vinculadas e não pode ser excluído.");

        repo.Remover(lead);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
