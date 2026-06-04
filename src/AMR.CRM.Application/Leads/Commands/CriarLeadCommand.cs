using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Leads.Commands;

public record CriarLeadCommand(
    string     Nome,
    string     Email,
    OrigemLead Origem,
    string?    Telefone      = null,
    string?    Empresa       = null,
    decimal    ValorEstimado = 0m,
    string?    Notas         = null
) : IRequest<Result<LeadDto>>;

public class CriarLeadCommandHandler(ILeadRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarLeadCommand, Result<LeadDto>>
{
    public async Task<Result<LeadDto>> Handle(CriarLeadCommand req, CancellationToken ct)
    {
        var lead = Lead.Criar(req.Nome, req.Email, req.Origem,
            req.Telefone, req.Empresa, req.ValorEstimado, req.Notas);

        await repo.AdicionarAsync(lead, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(ToDto(lead));
    }

    internal static LeadDto ToDto(Lead l) => new(
        l.Id, l.Nome, l.Email, l.Telefone, l.Empresa,
        l.Status, l.Status.ToString(),
        l.Origem, l.Origem.ToString(),
        l.ValorEstimado, l.Notas,
        l.CriadoEm, l.AlteradoEm
    );
}
