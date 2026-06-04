using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Interfaces;

namespace AMR.CRM.Application.Tests.Atividades;

internal class FakeAtividadeRepository : IAtividadeRepository
{
    public readonly List<Atividade> Store = [];

    public Task<Atividade?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(Store.FirstOrDefault(a => a.Id == id));

    public Task<List<Atividade>> ListarPorOportunidadeAsync(Guid oportunidadeId, CancellationToken ct = default)
        => Task.FromResult(Store.Where(a => a.OportunidadeId == oportunidadeId)
                                .OrderByDescending(a => a.DataHora).ToList());

    public Task AdicionarAsync(Atividade atividade, CancellationToken ct = default)
    {
        Store.Add(atividade);
        return Task.CompletedTask;
    }

    public void Atualizar(Atividade atividade) { }
    public void Remover(Atividade atividade) => Store.Remove(atividade);
}
