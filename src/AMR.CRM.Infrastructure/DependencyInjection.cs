using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Interfaces;
using AMR.CRM.Infrastructure.Data;
using AMR.CRM.Infrastructure.Data.Repositories;
using AMR.CRM.Infrastructure.Integrations;

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
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IOportunidadeRepository, OportunidadeRepository>();
        services.AddScoped<IAtividadeRepository, AtividadeRepository>();

        var coreBaseUrl = configuration["IntegracaoCore:BaseUrl"] ?? "http://localhost:5001";
        if (!int.TryParse(configuration["IntegracaoCore:Timeout"], out var coreTimeout))
            coreTimeout = 10;

        services.AddHttpClient<ICoreApiClient, CoreApiClient>(client =>
        {
            client.BaseAddress = new Uri(coreBaseUrl);
            client.Timeout     = TimeSpan.FromSeconds(coreTimeout);
        });

        return services;
    }
}
