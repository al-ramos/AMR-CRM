using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record AvancarOportunidadeCommand(Guid Id, StatusOportunidade NovoStatus) : IRequest<Result>;

public class AvancarOportunidadeCommandHandler(IOportunidadeRepository repo, IUnitOfWork uow)
    : IRequestHandler<AvancarOportunidadeCommand, Result>
{
    public async Task<Result> Handle(AvancarOportunidadeCommand req, CancellationToken ct)
    {
        var op = await repo.ObterPorIdAsync(req.Id, ct);
        if (op is null) return Result.Falha("Oportunidade não encontrada.");

        switch (req.NovoStatus)
        {
            case StatusOportunidade.EmAndamento: op.IniciarAndamento(); break;
            case StatusOportunidade.Ganha:       op.Ganhar();           break;
            case StatusOportunidade.Perdida:     op.Perder();           break;
            case StatusOportunidade.Cancelada:   op.Cancelar();         break;
            default: return Result.Falha("Transição de status inválida.");
        }

        repo.Atualizar(op);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
