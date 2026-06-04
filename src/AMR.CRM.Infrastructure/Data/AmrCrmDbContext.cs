using Microsoft.EntityFrameworkCore;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Infrastructure.Data;

public class AmrCrmDbContext(DbContextOptions<AmrCrmDbContext> options) : DbContext(options)
{
    public DbSet<Contato>      Contatos     => Set<Contato>();
    public DbSet<Oportunidade> Oportunidades => Set<Oportunidade>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(AmrCrmDbContext).Assembly);
        base.OnModelCreating(mb);
    }
}
