using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMR.CRM.Infrastructure.Data.Repositories;

public class AtividadeRepository(AmrCrmDbContext ctx) : IAtividadeRepository
{
    public Task<Atividade?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => ctx.Atividades
               .Include(a => a.Lead)
               .Include(a => a.Oportunidade)
               .FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<List<Atividade>> ListarAsync(
        StatusAtividade? status = null,
        TipoAtividade?   tipo   = null,
        CancellationToken ct    = default)
        => ctx.Atividades
               .Include(a => a.Lead)
               .Include(a => a.Oportunidade)
               .AsNoTracking()
               .Where(a => status == null || a.Status == status)
               .Where(a => tipo   == null || a.Tipo   == tipo)
               .OrderBy(a => a.DataPrevista)
               .ToListAsync(ct);

    public Task<List<Atividade>> ListarVencidasAsync(CancellationToken ct = default)
        => ctx.Atividades
               .Include(a => a.Lead)
               .Include(a => a.Oportunidade)
               .AsNoTracking()
               .Where(a => a.Status == StatusAtividade.Pendente && a.DataPrevista < DateTime.UtcNow)
               .OrderBy(a => a.DataPrevista)
               .ToListAsync(ct);

    public async Task AdicionarAsync(Atividade atividade, CancellationToken ct = default)
        => await ctx.Atividades.AddAsync(atividade, ct);

    public void Atualizar(Atividade atividade) => ctx.Atividades.Update(atividade);
    public void Remover(Atividade atividade)   => ctx.Atividades.Remove(atividade);
}
