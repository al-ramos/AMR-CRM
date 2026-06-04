using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Domain.Interfaces;

public interface IAtividadeRepository
{
    Task<Atividade?>       ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Atividade>>  ListarPorOportunidadeAsync(Guid oportunidadeId, CancellationToken ct = default);
    Task                   AdicionarAsync(Atividade atividade, CancellationToken ct = default);
    void                   Atualizar(Atividade atividade);
    void                   Remover(Atividade atividade);
}
