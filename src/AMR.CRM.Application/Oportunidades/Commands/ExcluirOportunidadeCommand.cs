using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record ExcluirOportunidadeCommand(Guid Id) : IRequest<Result>;

public class ExcluirOportunidadeCommandHandler(
    IOportunidadeRepository repo,
    IUnitOfWork uow)
    : IRequestHandler<ExcluirOportunidadeCommand, Result>
{
    public async Task<Result> Handle(ExcluirOportunidadeCommand req, CancellationToken ct)
    {
        var op = await repo.ObterPorIdAsync(req.Id, ct);
        if (op is null) return Result.Falha("Oportunidade não encontrada.");

        repo.Remover(op);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
