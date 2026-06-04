using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMR.CRM.Infrastructure.Data.Repositories;

public class ContatoRepository(AmrCrmDbContext ctx) : IContatoRepository
{
    public Task<Contato?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => ctx.Contatos.Include(c => c.Oportunidades)
               .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Contato>> ListarAsync(CancellationToken ct = default)
        => ctx.Contatos.AsNoTracking().OrderBy(c => c.Nome).ToListAsync(ct);

    public async Task AdicionarAsync(Contato contato, CancellationToken ct = default)
        => await ctx.Contatos.AddAsync(contato, ct);

    public void Atualizar(Contato contato)
        => ctx.Contatos.Update(contato);
}
