using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Oportunidades.Queries;

public record ListarOportunidadesQuery : IRequest<List<OportunidadeDto>>;

public class ListarOportunidadesQueryHandler(IOportunidadeRepository repo)
    : IRequestHandler<ListarOportunidadesQuery, List<OportunidadeDto>>
{
    public async Task<List<OportunidadeDto>> Handle(ListarOportunidadesQuery request, CancellationToken ct)
    {
        var lista = await repo.ListarAsync(ct);
        return lista.Select(ToDto).ToList();
    }

    private static OportunidadeDto ToDto(Oportunidade o) => new(
        o.Id, o.ContatoId,
        o.Contato?.Nome ?? "-",
        o.Titulo, o.Valor,
        o.Status, o.Status.ToString(),
        o.Descricao, o.PrevisaoFechamento, o.CriadoEm
    );
}
