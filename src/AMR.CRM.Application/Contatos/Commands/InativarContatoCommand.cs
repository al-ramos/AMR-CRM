using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Application.Contatos.Commands;

public record InativarContatoCommand(Guid Id) : IRequest<Result>;

public class InativarContatoCommandHandler(IContatoRepository repo, IUnitOfWork uow)
    : IRequestHandler<InativarContatoCommand, Result>
{
    public async Task<Result> Handle(InativarContatoCommand req, CancellationToken ct)
    {
        var contato = await repo.ObterPorIdAsync(req.Id, ct);
        if (contato is null) return Result.Falha("Contato não encontrado.");

        contato.Inativar();
        repo.Atualizar(contato);
        await uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
