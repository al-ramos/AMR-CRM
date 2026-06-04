using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Leads.Queries;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Leads.Commands;

public record CriarLeadCommand(
    string     Nome,
    string     Email,
    OrigemLead Origem,
    decimal    ValorEstimado = 0,
    string?    Telefone      = null,
    string?    Empresa       = null,
    string?    Notas         = null
) : IRequest<Result<LeadDto>>;

public class CriarLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarLeadCommand, Result<LeadDto>>
{
    public async Task<Result<LeadDto>> Handle(CriarLeadCommand req, CancellationToken ct)
    {
        var lead = Lead.Criar(req.Nome, req.Email, req.Origem,
            req.ValorEstimado, req.Telefone, req.Empresa, req.Notas);
        await repo.AdicionarAsync(lead, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(ListarLeadsQueryHandler.ToDto(lead));
    }
}
