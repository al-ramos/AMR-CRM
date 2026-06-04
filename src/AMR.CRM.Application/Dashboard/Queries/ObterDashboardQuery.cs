using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Dashboard.Queries;

public record ObterDashboardQuery : IRequest<Result<DashboardDto>>;

public class ObterDashboardQueryHandler(
    IContatoRepository     contatoRepo,
    IOportunidadeRepository opRepo)
    : IRequestHandler<ObterDashboardQuery, Result<DashboardDto>>
{
    private static readonly (StatusOportunidade Status, string Nome)[] _colunas =
    [
        (StatusOportunidade.Aberta,       "Aberta"),
        (StatusOportunidade.EmAndamento,  "Em Andamento"),
        (StatusOportunidade.Ganha,        "Ganha"),
        (StatusOportunidade.Perdida,      "Perdida"),
        (StatusOportunidade.Cancelada,    "Cancelada"),
    ];

    private static readonly string[] _mesesPt =
        ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"];

    public async Task<Result<DashboardDto>> Handle(ObterDashboardQuery _, CancellationToken ct)
    {
        var contatos     = await contatoRepo.ListarAsync(ct);
        var oportunidades = await opRepo.ListarAsync(ct);

        var ganhas       = oportunidades.Where(o => o.Status == StatusOportunidade.Ganha).ToList();
        var pipeline     = oportunidades
            .Where(o => o.Status is StatusOportunidade.Aberta or StatusOportunidade.EmAndamento)
            .Sum(o => o.Valor);
        var taxaConversao = oportunidades.Count > 0
            ? Math.Round((decimal)ganhas.Count / oportunidades.Count * 100, 1)
            : 0m;
        var ticketMedio  = ganhas.Count > 0
            ? Math.Round(ganhas.Average(o => o.Valor), 2)
            : 0m;

        var kanban = _colunas.Select(col =>
        {
            var grupo = oportunidades.Where(o => o.Status == col.Status).ToList();
            var cards = grupo.Take(10).Select(o => new KanbanCardDto(
                o.Id, o.Titulo, o.Contato?.Nome ?? "-", o.Valor)).ToList();
            return new KanbanColunaDto(
                (int)col.Status, col.Nome,
                grupo.Count, grupo.Sum(o => o.Valor),
                cards);
        }).ToList();

        var hoje   = DateTime.UtcNow;
        var meses  = Enumerable.Range(0, 6)
            .Select(i => hoje.AddMonths(-5 + i))
            .Select(d => new { d.Year, d.Month, Label = _mesesPt[d.Month - 1] })
            .ToList();

        var porMes = meses.Select(m =>
        {
            var grupo = oportunidades
                .Where(o => o.CriadoEm.Year == m.Year && o.CriadoEm.Month == m.Month)
                .ToList();
            return new OportunidadesMesDto(m.Label, grupo.Count, grupo.Sum(o => o.Valor));
        }).ToList();

        return Result.Ok(new DashboardDto(
            contatos.Count,
            oportunidades.Count,
            pipeline,
            taxaConversao,
            ticketMedio,
            kanban,
            porMes));
    }
}
