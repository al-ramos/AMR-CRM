using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Leads.Commands;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Leads.Queries;

public record ListarLeadsQuery(StatusLead? Status = null) : IRequest<List<LeadDto>>;

public class ListarLeadsQueryHandler(ILeadRepository repo)
    : IRequestHandler<ListarLeadsQuery, List<LeadDto>>
{
    public async Task<List<LeadDto>> Handle(ListarLeadsQuery request, CancellationToken ct)
    {
        var leads = request.Status.HasValue
            ? await repo.ListarPorStatusAsync(request.Status.Value, ct)
            : await repo.ListarAsync(ct);

        return leads.Select(CriarLeadCommandHandler.ToDto).ToList();
    }
}
