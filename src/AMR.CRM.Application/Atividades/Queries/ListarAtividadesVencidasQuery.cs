using AMR.CRM.Application.Atividades.Commands;
using AMR.CRM.Application.DTOs;

namespace AMR.CRM.Application.Atividades.Queries;

public record ListarAtividadesVencidasQuery : IRequest<List<AtividadeDto>>;

public class ListarAtividadesVencidasQueryHandler(IAtividadeRepository repo)
    : IRequestHandler<ListarAtividadesVencidasQuery, List<AtividadeDto>>
{
    public async Task<List<AtividadeDto>> Handle(ListarAtividadesVencidasQuery req, CancellationToken ct)
    {
        var lista = await repo.ListarVencidasAsync(ct);
        return lista.Select(CriarAtividadeCommandHandler.ToDto).ToList();
    }
}
