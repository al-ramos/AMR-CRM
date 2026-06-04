using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Atividades.Queries;

public record ListarAtividadesQuery(Guid OportunidadeId) : IRequest<List<AtividadeDto>>;

public class ListarAtividadesQueryHandler(IAtividadeRepository repo)
    : IRequestHandler<ListarAtividadesQuery, List<AtividadeDto>>
{
    public async Task<List<AtividadeDto>> Handle(ListarAtividadesQuery req, CancellationToken ct)
    {
        var lista = await repo.ListarPorOportunidadeAsync(req.OportunidadeId, ct);
        return lista.Select(ToDto).ToList();
    }

    internal static AtividadeDto ToDto(Atividade a) => new(
        a.Id, a.OportunidadeId, a.Tipo, a.Tipo.ToString(),
        a.Descricao, a.DataHora, a.Concluida, a.CriadoEm
    );
}
