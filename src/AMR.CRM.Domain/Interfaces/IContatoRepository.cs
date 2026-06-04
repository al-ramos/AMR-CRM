using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Domain.Interfaces;

public interface IContatoRepository
{
    Task<Contato?>            ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Contato>>       ListarAsync(CancellationToken ct = default);
    Task                      AdicionarAsync(Contato contato, CancellationToken ct = default);
    void                      Atualizar(Contato contato);
}
