using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Application.Atividades.Commands;

public record ExcluirAtividadeCommand(Guid Id) : IRequest<Result>;

public class ExcluirAtividadeCommandHandler(IAtividadeRepository repo, IUnitOfWork uow)
    : IRequestHandler<ExcluirAtividadeCommand, Result>
{
    public async Task<Result> Handle(ExcluirAtividadeCommand req, CancellationToken ct)
    {
        var atividade = await repo.ObterPorIdAsync(req.Id, ct);
        if (atividade is null) return Result.Falha("Atividade não encontrada.");

        repo.Remover(atividade);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
