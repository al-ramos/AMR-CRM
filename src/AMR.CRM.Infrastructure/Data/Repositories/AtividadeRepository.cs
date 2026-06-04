using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMR.CRM.Infrastructure.Data.Repositories;

public class AtividadeRepository(AmrCrmDbContext ctx) : IAtividadeRepository
{
    public Task<Atividade?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => ctx.Atividades.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<List<Atividade>> ListarPorOportunidadeAsync(Guid oportunidadeId, CancellationToken ct = default)
        => ctx.Atividades
               .Where(a => a.OportunidadeId == oportunidadeId)
               .AsNoTracking()
               .OrderByDescending(a => a.DataHora)
               .ToListAsync(ct);

    public async Task AdicionarAsync(Atividade atividade, CancellationToken ct = default)
        => await ctx.Atividades.AddAsync(atividade, ct);

    public void Atualizar(Atividade atividade) => ctx.Atividades.Update(atividade);

    public void Remover(Atividade atividade) => ctx.Atividades.Remove(atividade);
}
