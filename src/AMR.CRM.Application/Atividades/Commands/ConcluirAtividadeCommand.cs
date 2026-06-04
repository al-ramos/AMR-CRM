using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Application.Atividades.Commands;

public record ConcluirAtividadeCommand(Guid Id) : IRequest<Result>;

public class ConcluirAtividadeCommandHandler(
    IAtividadeRepository repo,
    IUnitOfWork          uow)
    : IRequestHandler<ConcluirAtividadeCommand, Result>
{
    public async Task<Result> Handle(ConcluirAtividadeCommand req, CancellationToken ct)
    {
        var a = await repo.ObterPorIdAsync(req.Id, ct);
        if (a is null) return Result.Falha("Atividade não encontrada.");

        a.ToggleConcluida();
        repo.Atualizar(a);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
