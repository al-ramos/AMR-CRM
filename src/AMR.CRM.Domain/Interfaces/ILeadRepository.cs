using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Interfaces;

public interface ILeadRepository
{
    Task<Lead?>          ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Lead>>     ListarAsync(CancellationToken ct = default);
    Task<List<Lead>>     ListarPorStatusAsync(StatusLead status, CancellationToken ct = default);
    Task<List<int>>      ListarOrigemCoreClienteIdsAsync(CancellationToken ct = default);
    Task                 AdicionarAsync(Lead lead, CancellationToken ct = default);
    void                 Atualizar(Lead lead);
    void                 Remover(Lead lead);
}
