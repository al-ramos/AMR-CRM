using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Interfaces;
using AMR.CRM.Infrastructure.Data;
using AMR.CRM.Infrastructure.Data.Repositories;

namespace AMR.CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AmrCrmDbContext>(opts =>
            opts.UseSqlite(
                configuration.GetConnectionString("AmrCrm"),
                sql => sql.MigrationsAssembly(typeof(AmrCrmDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IContatoRepository, ContatoRepository>();
        services.AddScoped<IOportunidadeRepository, OportunidadeRepository>();

        return services;
    }
}
