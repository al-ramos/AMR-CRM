using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Contatos.Commands;

public record CriarContatoCommand(
    string      Nome,
    string      Email,
    TipoContato Tipo,
    string?     Telefone = null,
    string?     Empresa  = null,
    string?     Notas    = null
) : IRequest<Result<ContatoDto>>;

public class CriarContatoCommandHandler(IContatoRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarContatoCommand, Result<ContatoDto>>
{
    public async Task<Result<ContatoDto>> Handle(CriarContatoCommand req, CancellationToken ct)
    {
        var contato = Contato.Criar(req.Nome, req.Email, req.Tipo, req.Telefone, req.Empresa, req.Notas);
        await repo.AdicionarAsync(contato, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(new ContatoDto(
            contato.Id, contato.Nome, contato.Email, contato.Telefone, contato.Empresa,
            contato.Tipo,   contato.Tipo.ToString(),
            contato.Status, contato.Status.ToString(),
            contato.Notas,  contato.CriadoEm
        ));
    }
}
