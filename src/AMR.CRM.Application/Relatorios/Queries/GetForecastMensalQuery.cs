using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Relatorios.Queries;

public record GetForecastMensalQuery(string Mes) : IRequest<ForecastDto>;

public class GetForecastMensalQueryHandler(IOportunidadeRepository opRepo)
    : IRequestHandler<GetForecastMensalQuery, ForecastDto>
{
    public async Task<ForecastDto> Handle(GetForecastMensalQuery request, CancellationToken ct)
    {
        if (!DateTime.TryParse(request.Mes + "-01", out var mesRef))
            mesRef = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var inicio = new DateTime(mesRef.Year, mesRef.Month, 1);
        var fim    = inicio.AddMonths(1);

        var todas = await opRepo.ListarAsync(ct);
        var ops   = todas
            .Where(o => o.PrevisaoFechamento.HasValue
                     && o.PrevisaoFechamento.Value >= inicio
                     && o.PrevisaoFechamento.Value < fim
                     && o.Status != StatusOportunidade.Cancelada)
            .ToList();

        var receita  = ops
            .Where(o => o.Status != StatusOportunidade.Ganha)
            .Sum(o => o.Valor * o.Probabilidade / 100m);

        var confirmada = ops
            .Where(o => o.Status == StatusOportunidade.Ganha)
            .Sum(o => o.Valor);

        var itens = ops.Select(o => new ForecastOportunidadeDto(
            o.Id,
            o.Titulo,
            o.Valor,
            o.Probabilidade,
            Math.Round(o.Valor * o.Probabilidade / 100m, 2),
            o.PrevisaoFechamento!.Value,
            o.Status.ToString(),
            o.Etapa.ToString()
        )).OrderByDescending(o => o.ValorPonderado).ToList();

        return new ForecastDto(
            request.Mes,
            Math.Round(receita, 2),
            Math.Round(confirmada, 2),
            ops.Count,
            itens);
    }
}
