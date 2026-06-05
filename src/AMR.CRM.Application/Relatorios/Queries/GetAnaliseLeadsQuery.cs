using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Relatorios.Queries;

public record GetAnaliseLeadsQuery(int Periodo = 30) : IRequest<AnaliseLeadsDto>;

public class GetAnaliseLeadsQueryHandler(ILeadRepository leadRepo)
    : IRequestHandler<GetAnaliseLeadsQuery, AnaliseLeadsDto>
{
    public async Task<AnaliseLeadsDto> Handle(GetAnaliseLeadsQuery request, CancellationToken ct)
    {
        var corte = DateTime.UtcNow.AddDays(-request.Periodo);
        var leads = (await leadRepo.ListarAsync(ct))
            .Where(l => l.CriadoEm >= corte)
            .ToList();

        var porOrigem = Enum.GetValues<OrigemLead>()
            .Select(o => new LeadsPorOrigemItemDto(
                o.ToString(),
                leads.Count(l => l.Origem == o),
                leads.Where(l => l.Origem == o).Sum(l => l.ValorEstimado)))
            .Where(x => x.Total > 0)
            .OrderByDescending(x => x.Total)
            .ToList();

        var porStatus = Enum.GetValues<StatusLead>()
            .Select(s => new LeadsPorStatusItemDto(
                s.ToString(),
                leads.Count(l => l.Status == s),
                leads.Where(l => l.Status == s).Sum(l => l.ValorEstimado)))
            .Where(x => x.Total > 0)
            .ToList();

        return new AnaliseLeadsDto(
            porOrigem,
            porStatus,
            leads.Count,
            leads.Sum(l => l.ValorEstimado),
            request.Periodo);
    }
}
