using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Interfaces;

public interface IAtividadeRepository
{
    Task<Atividade?>      ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Atividade>> ListarAsync(StatusAtividade? status = null, TipoAtividade? tipo = null, CancellationToken ct = default);
    Task<List<Atividade>> ListarVencidasAsync(CancellationToken ct = default);
    Task                  AdicionarAsync(Atividade atividade, CancellationToken ct = default);
    void                  Atualizar(Atividade atividade);
    void                  Remover(Atividade atividade);
}
