using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Contatos.Commands;

public record AtualizarContatoCommand(
    Guid        Id,
    string      Nome,
    string      Email,
    TipoContato Tipo,
    string?     Telefone = null,
    string?     Empresa  = null,
    string?     Notas    = null
) : IRequest<Result>;

public class AtualizarContatoCommandHandler(IContatoRepository repo, IUnitOfWork uow)
    : IRequestHandler<AtualizarContatoCommand, Result>
{
    public async Task<Result> Handle(AtualizarContatoCommand req, CancellationToken ct)
    {
        var contato = await repo.ObterPorIdAsync(req.Id, ct);
        if (contato is null) return Result.Falha("Contato não encontrado.");

        contato.Atualizar(req.Nome, req.Email, req.Tipo, req.Telefone, req.Empresa, req.Notas);
        repo.Atualizar(contato);
        await uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
