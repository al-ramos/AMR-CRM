using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMR.CRM.Infrastructure.Data.Repositories;

public class LeadRepository(AmrCrmDbContext ctx) : ILeadRepository
{
    public Task<Lead?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => ctx.Leads.Include(l => l.Oportunidades)
               .FirstOrDefaultAsync(l => l.Id == id, ct);

    public Task<List<Lead>> ListarAsync(CancellationToken ct = default)
        => ctx.Leads.AsNoTracking()
               .OrderByDescending(l => l.CriadoEm)
               .ToListAsync(ct);

    public Task<List<Lead>> ListarPorStatusAsync(StatusLead status, CancellationToken ct = default)
        => ctx.Leads.AsNoTracking()
               .Where(l => l.Status == status)
               .OrderByDescending(l => l.CriadoEm)
               .ToListAsync(ct);

    public Task<List<int>> ListarOrigemCoreClienteIdsAsync(CancellationToken ct = default)
        => ctx.Leads.AsNoTracking()
               .Where(l => l.OrigemCoreClienteId.HasValue)
               .Select(l => l.OrigemCoreClienteId!.Value)
               .ToListAsync(ct);

    public async Task AdicionarAsync(Lead lead, CancellationToken ct = default)
        => await ctx.Leads.AddAsync(lead, ct);

    public void Atualizar(Lead lead)  => ctx.Leads.Update(lead);
    public void Remover(Lead lead)    => ctx.Leads.Remove(lead);
}
