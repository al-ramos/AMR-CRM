using AMR.CRM.Application.DTOs;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Relatorios.Queries;

public record GetFunilConversaoQuery(int Periodo = 30) : IRequest<FunilConversaoDto>;

public class GetFunilConversaoQueryHandler(ILeadRepository leadRepo)
    : IRequestHandler<GetFunilConversaoQuery, FunilConversaoDto>
{
    public async Task<FunilConversaoDto> Handle(GetFunilConversaoQuery request, CancellationToken ct)
    {
        var corte = DateTime.UtcNow.AddDays(-request.Periodo);
        var leads = (await leadRepo.ListarAsync(ct))
            .Where(l => l.CriadoEm >= corte)
            .ToList();

        var total = leads.Count;

        var etapas = new[]
        {
            StatusLead.Novo,
            StatusLead.Qualificado,
            StatusLead.Proposta,
            StatusLead.Ganho,
        };

        var itens = etapas.Select(status =>
        {
            var count = leads.Count(l => l.Status == status);
            var valor = leads.Where(l => l.Status == status).Sum(l => l.ValorEstimado);
            var taxa  = total > 0 ? Math.Round(count * 100m / total, 1) : 0m;
            return new FunilConversaoItemDto(status.ToString(), count, valor, taxa);
        }).ToList();

        var taxaGeral = total > 0
            ? Math.Round(leads.Count(l => l.Status == StatusLead.Ganho) * 100m / total, 1)
            : 0m;

        return new FunilConversaoDto(itens, taxaGeral, request.Periodo);
    }
}
