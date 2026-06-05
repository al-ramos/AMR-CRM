using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Dashboard.Queries;

public record GetTopOportunidadesQuery(int Top = 5) : IRequest<List<TopOportunidadeDto>>;

public class GetTopOportunidadesQueryHandler(IOportunidadeRepository repo)
    : IRequestHandler<GetTopOportunidadesQuery, List<TopOportunidadeDto>>
{
    public async Task<List<TopOportunidadeDto>> Handle(GetTopOportunidadesQuery request, CancellationToken ct)
    {
        var ops = await repo.ListarAsync(ct);
        return ops
            .Where(o => o.Status != StatusOportunidade.Cancelada)
            .OrderByDescending(o => o.Valor)
            .Take(request.Top)
            .Select(o => new TopOportunidadeDto(
                o.Id,
                o.Titulo,
                o.Lead?.Nome,
                o.Contato?.Nome,
                o.Valor,
                o.Probabilidade,
                Math.Round(o.Valor * o.Probabilidade / 100m, 2),
                o.Etapa.ToString(),
                o.Status.ToString()))
            .ToList();
    }
}
