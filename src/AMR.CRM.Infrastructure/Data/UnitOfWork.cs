using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Infrastructure.Data;

public class UnitOfWork(AmrCrmDbContext ctx) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => ctx.SaveChangesAsync(ct);
}
