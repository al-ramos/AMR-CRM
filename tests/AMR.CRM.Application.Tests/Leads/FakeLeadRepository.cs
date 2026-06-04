using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Interfaces;

namespace AMR.CRM.Application.Tests.Leads;

internal class FakeLeadRepository : ILeadRepository
{
    public readonly List<Lead> Store = [];

    public Task<Lead?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(Store.FirstOrDefault(l => l.Id == id));

    public Task<List<Lead>> ListarAsync(CancellationToken ct = default)
        => Task.FromResult(Store.ToList());

    public Task AdicionarAsync(Lead lead, CancellationToken ct = default)
    {
        Store.Add(lead);
        return Task.CompletedTask;
    }

    public void Atualizar(Lead lead) { }
    public void Remover(Lead lead) => Store.Remove(lead);
}

internal class FakeUnitOfWork : AMR.CRM.Application.Interfaces.IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}
