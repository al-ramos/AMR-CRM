using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Dashboard.Queries;

public record GetFunilQuery : IRequest<FunilDto>;

public class GetFunilQueryHandler(ILeadRepository leadRepo, IOportunidadeRepository opRepo)
    : IRequestHandler<GetFunilQuery, FunilDto>
{
    public async Task<FunilDto> Handle(GetFunilQuery request, CancellationToken ct)
    {
        var leads = await leadRepo.ListarAsync(ct);
        var ops   = await opRepo.ListarAsync(ct);

        var leadItems = Enum.GetValues<StatusLead>()
            .Select(s => new FunilLeadItemDto(
                s.ToString(),
                leads.Count(l => l.Status == s),
                leads.Where(l => l.Status == s).Sum(l => l.ValorEstimado)))
            .ToList();

        var etapaItems = Enum.GetValues<EtapaOportunidade>()
            .Select(e => new FunilEtapaItemDto(
                e.ToString(),
                ops.Count(o => o.Etapa == e),
                ops.Where(o => o.Etapa == e).Sum(o => o.Valor)))
            .ToList();

        return new FunilDto(leadItems, etapaItems);
    }
}
