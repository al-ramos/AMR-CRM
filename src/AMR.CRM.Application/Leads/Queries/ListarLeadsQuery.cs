using AMR.CRM.Application.DTOs;

namespace AMR.CRM.Application.Leads.Queries;

public record ListarLeadsQuery : IRequest<Result<List<LeadDto>>>;

public class ListarLeadsQueryHandler(ILeadRepository repo)
    : IRequestHandler<ListarLeadsQuery, Result<List<LeadDto>>>
{
    public async Task<Result<List<LeadDto>>> Handle(ListarLeadsQuery _, CancellationToken ct)
    {
        var leads = await repo.ListarAsync(ct);
        return Result.Ok(leads.Select(ToDto).ToList());
    }

    internal static LeadDto ToDto(AMR.CRM.Domain.Entities.Lead l) => new(
        l.Id, l.Nome, l.Email, l.Telefone, l.Empresa,
        l.Status, l.Status.ToString(),
        l.Origem, l.Origem.ToString(),
        l.ValorEstimado, l.Notas, l.CriadoEm);
}
