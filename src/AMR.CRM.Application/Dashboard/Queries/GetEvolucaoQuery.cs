using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;
using System.Globalization;

namespace AMR.CRM.Application.Dashboard.Queries;

public record GetEvolucaoQuery(int Periodo = 30) : IRequest<List<EvolucaoItemDto>>;

public class GetEvolucaoQueryHandler(IOportunidadeRepository repo)
    : IRequestHandler<GetEvolucaoQuery, List<EvolucaoItemDto>>
{
    public async Task<List<EvolucaoItemDto>> Handle(GetEvolucaoQuery request, CancellationToken ct)
    {
        var ops   = await repo.ListarAsync(ct);
        var hoje  = DateTime.UtcNow;
        var desde = hoje.AddDays(-request.Periodo);

        var inicio = new DateTime(desde.Year, desde.Month, 1);
        var fim    = new DateTime(hoje.Year, hoje.Month, 1);
        var ptBR   = new CultureInfo("pt-BR");

        var meses = new List<DateTime>();
        for (var m = inicio; m <= fim; m = m.AddMonths(1))
            meses.Add(m);

        return meses.Select(m => new EvolucaoItemDto(
            m.ToString("MMM/yy", ptBR),
            ops.Count(o => o.CriadoEm.Year == m.Year && o.CriadoEm.Month == m.Month),
            ops.Count(o => (o.Status == StatusOportunidade.Ganha || o.Status == StatusOportunidade.Perdida)
                        && o.AlteradoEm.Year == m.Year && o.AlteradoEm.Month == m.Month)))
            .ToList();
    }
}
