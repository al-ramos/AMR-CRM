using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Leads.Queries;

namespace AMR.CRM.Application.Leads.Queries;

public record ObterLeadQuery(Guid Id) : IRequest<Result<LeadDto>>;

public class ObterLeadQueryHandler(ILeadRepository repo)
    : IRequestHandler<ObterLeadQuery, Result<LeadDto>>
{
    public async Task<Result<LeadDto>> Handle(ObterLeadQuery req, CancellationToken ct)
    {
        var lead = await repo.ObterPorIdAsync(req.Id, ct);
        if (lead is null) return Result.Falha<LeadDto>("Lead não encontrado.");
        return Result.Ok(ListarLeadsQueryHandler.ToDto(lead));
    }
}
