using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMR.CRM.Infrastructure.Data.Repositories;

public class OportunidadeRepository(AmrCrmDbContext ctx) : IOportunidadeRepository
{
    public Task<Oportunidade?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => ctx.Oportunidades
               .Include(o => o.Contato)
               .Include(o => o.Lead)
               .FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<List<Oportunidade>> ListarAsync(CancellationToken ct = default)
        => ctx.Oportunidades
               .Include(o => o.Contato)
               .Include(o => o.Lead)
               .AsNoTracking()
               .OrderByDescending(o => o.CriadoEm)
               .ToListAsync(ct);

    public Task<List<Oportunidade>> ListarPorContatoAsync(Guid contatoId, CancellationToken ct = default)
        => ctx.Oportunidades
               .Include(o => o.Contato)
               .Include(o => o.Lead)
               .Where(o => o.ContatoId == contatoId)
               .AsNoTracking()
               .ToListAsync(ct);

    public async Task AdicionarAsync(Oportunidade oportunidade, CancellationToken ct = default)
        => await ctx.Oportunidades.AddAsync(oportunidade, ct);

    public void Atualizar(Oportunidade oportunidade)
        => ctx.Oportunidades.Update(oportunidade);
}
