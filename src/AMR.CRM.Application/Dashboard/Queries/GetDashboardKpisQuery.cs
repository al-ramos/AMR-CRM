using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Dashboard.Queries;

public record GetDashboardKpisQuery : IRequest<DashboardKpisDto>;

public class GetDashboardKpisQueryHandler(ILeadRepository leadRepo, IOportunidadeRepository opRepo)
    : IRequestHandler<GetDashboardKpisQuery, DashboardKpisDto>
{
    public async Task<DashboardKpisDto> Handle(GetDashboardKpisQuery request, CancellationToken ct)
    {
        var leads = await leadRepo.ListarAsync(ct);
        var ops   = await opRepo.ListarAsync(ct);

        var leadsAtivos  = leads.Count(l => l.Status != StatusLead.Ganho && l.Status != StatusLead.Perdido);
        var opAbertas    = ops.Count(o => o.Status == StatusOportunidade.Aberta || o.Status == StatusOportunidade.EmAndamento);
        var pipelinePond = ops
            .Where(o => o.Status == StatusOportunidade.Aberta || o.Status == StatusOportunidade.EmAndamento)
            .Sum(o => o.Valor * o.Probabilidade / 100m);
        var taxaConversao = leads.Count > 0
            ? Math.Round(leads.Count(l => l.Status == StatusLead.Ganho) * 100m / leads.Count, 1)
            : 0m;

        return new DashboardKpisDto(leads.Count, leadsAtivos, opAbertas, pipelinePond, taxaConversao);
    }
}
