using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Leads.Commands;

public record ImportarLeadsFromCoreCommand(List<int> ClienteIds) : IRequest<Result<ImportacaoResultDto>>;

public class ImportarLeadsFromCoreCommandHandler(
    ICoreApiClient  coreApiClient,
    ILeadRepository repo,
    IUnitOfWork     uow)
    : IRequestHandler<ImportarLeadsFromCoreCommand, Result<ImportacaoResultDto>>
{
    public async Task<Result<ImportacaoResultDto>> Handle(
        ImportarLeadsFromCoreCommand req, CancellationToken ct)
    {
        if (req.ClienteIds.Count == 0)
            return Result.Ok(new ImportacaoResultDto(0, 0));

        var existingIds = await repo.ListarOrigemCoreClienteIdsAsync(ct);
        var existingSet = existingIds.ToHashSet();

        List<ClienteCoreDto> clientes;
        try
        {
            clientes = await coreApiClient.GetClientesAsync(ct);
        }
        catch (CoreApiUnavailableException ex)
        {
            return Result.Falha<ImportacaoResultDto>(ex.Message);
        }

        var clienteMap = clientes.ToDictionary(c => c.Id);

        int importados = 0;
        int ignorados  = 0;

        foreach (var clienteId in req.ClienteIds)
        {
            if (existingSet.Contains(clienteId) || !clienteMap.TryGetValue(clienteId, out var c))
            {
                ignorados++;
                continue;
            }

            var lead = Lead.Criar(
                nome:               c.Nome,
                email:              c.Email,
                origem:             OrigemLead.Outro,
                telefone:           c.Telefone,
                origemCoreClienteId: clienteId);

            await repo.AdicionarAsync(lead, ct);
            importados++;
        }

        if (importados > 0)
            await uow.SaveChangesAsync(ct);

        return Result.Ok(new ImportacaoResultDto(importados, ignorados));
    }
}
