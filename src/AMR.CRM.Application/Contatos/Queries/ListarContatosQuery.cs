using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Contatos.Queries;

public record ListarContatosQuery : IRequest<List<ContatoDto>>;

public class ListarContatosQueryHandler(IContatoRepository repo)
    : IRequestHandler<ListarContatosQuery, List<ContatoDto>>
{
    public async Task<List<ContatoDto>> Handle(ListarContatosQuery request, CancellationToken ct)
    {
        var contatos = await repo.ListarAsync(ct);
        return contatos.Select(ToDto).ToList();
    }

    private static ContatoDto ToDto(Contato c) => new(
        c.Id, c.Nome, c.Email, c.Telefone, c.Empresa,
        c.Tipo,   c.Tipo.ToString(),
        c.Status, c.Status.ToString(),
        c.Notas,  c.CriadoEm
    );
}
