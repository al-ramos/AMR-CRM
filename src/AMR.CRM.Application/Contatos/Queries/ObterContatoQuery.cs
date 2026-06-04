using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Contatos.Queries;

public record ObterContatoQuery(Guid Id) : IRequest<ContatoDto?>;

public class ObterContatoQueryHandler(IContatoRepository repo)
    : IRequestHandler<ObterContatoQuery, ContatoDto?>
{
    public async Task<ContatoDto?> Handle(ObterContatoQuery request, CancellationToken ct)
    {
        var c = await repo.ObterPorIdAsync(request.Id, ct);
        if (c is null) return null;

        return new ContatoDto(
            c.Id, c.Nome, c.Email, c.Telefone, c.Empresa,
            c.Tipo,   c.Tipo.ToString(),
            c.Status, c.Status.ToString(),
            c.Notas,  c.CriadoEm
        );
    }
}
