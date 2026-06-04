using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Leads.Commands;

namespace AMR.CRM.Application.Leads.Queries;

public record ObterLeadQuery(Guid Id) : IRequest<LeadDto?>;

public class ObterLeadQueryHandler(ILeadRepository repo)
    : IRequestHandler<ObterLeadQuery, LeadDto?>
{
    public async Task<LeadDto?> Handle(ObterLeadQuery request, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(request.Id, ct);
        return lead is null ? null : CriarLeadCommandHandler.ToDto(lead);
    }
}
