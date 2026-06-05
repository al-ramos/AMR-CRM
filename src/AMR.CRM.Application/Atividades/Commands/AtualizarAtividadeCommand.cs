using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Atividades.Commands;

public record AtualizarAtividadeCommand(
    Guid            Id,
    string          Titulo,
    TipoAtividade   Tipo,
    DateTime        DataPrevista,
    StatusAtividade Status,
    string?         Observacao = null
) : IRequest<Result>;

public class AtualizarAtividadeCommandHandler(IAtividadeRepository repo, IUnitOfWork uow)
    : IRequestHandler<AtualizarAtividadeCommand, Result>
{
    public async Task<Result> Handle(AtualizarAtividadeCommand req, CancellationToken ct)
    {
        var atividade = await repo.ObterPorIdAsync(req.Id, ct);
        if (atividade is null) return Result.Falha("Atividade não encontrada.");

        atividade.Atualizar(req.Titulo, req.Tipo, req.DataPrevista, req.Status, req.Observacao);
        repo.Atualizar(atividade);
        await uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
