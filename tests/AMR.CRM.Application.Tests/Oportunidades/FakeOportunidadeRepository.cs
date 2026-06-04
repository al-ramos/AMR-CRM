using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Interfaces;

namespace AMR.CRM.Application.Tests.Oportunidades;

internal class FakeOportunidadeRepository : IOportunidadeRepository
{
    public readonly List<Oportunidade> Store = [];

    public Task<Oportunidade?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(Store.FirstOrDefault(o => o.Id == id));

    public Task<List<Oportunidade>> ListarAsync(CancellationToken ct = default)
        => Task.FromResult(Store.ToList());

    public Task<List<Oportunidade>> ListarPorContatoAsync(Guid contatoId, CancellationToken ct = default)
        => Task.FromResult(Store.Where(o => o.ContatoId == contatoId).ToList());

    public Task AdicionarAsync(Oportunidade oportunidade, CancellationToken ct = default)
    {
        Store.Add(oportunidade);
        return Task.CompletedTask;
    }

    public void Atualizar(Oportunidade oportunidade) { }
    public void Remover(Oportunidade oportunidade) => Store.Remove(oportunidade);
}
