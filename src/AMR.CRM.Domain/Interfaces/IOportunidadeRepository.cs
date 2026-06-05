using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Domain.Interfaces;

public interface IOportunidadeRepository
{
    Task<Oportunidade?>       ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Oportunidade>>  ListarAsync(CancellationToken ct = default);
    Task<List<Oportunidade>>  ListarPorContatoAsync(Guid contatoId, CancellationToken ct = default);
    Task                      AdicionarAsync(Oportunidade oportunidade, CancellationToken ct = default);
    void                      Atualizar(Oportunidade oportunidade);
    void                      Remover(Oportunidade oportunidade);
}
