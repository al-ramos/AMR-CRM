using AMR.CRM.Application.Atividades.Commands;
using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Atividades.Queries;

public record ListarAtividadesQuery(
    StatusAtividade? Status = null,
    TipoAtividade?   Tipo   = null
) : IRequest<List<AtividadeDto>>;

public class ListarAtividadesQueryHandler(IAtividadeRepository repo)
    : IRequestHandler<ListarAtividadesQuery, List<AtividadeDto>>
{
    public async Task<List<AtividadeDto>> Handle(ListarAtividadesQuery req, CancellationToken ct)
    {
        var lista = await repo.ListarAsync(req.Status, req.Tipo, ct);
        return lista.Select(CriarAtividadeCommandHandler.ToDto).ToList();
    }
}
