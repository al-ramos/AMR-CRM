using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        IConfiguration configuration,
        IHostEnvironment environment)
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

        var coreBaseUrl = UrlDoServico(configuration, environment, "IntegracaoCore:BaseUrl", "http://localhost:5001");
        if (!int.TryParse(configuration["IntegracaoCore:Timeout"], out var coreTimeout))
            coreTimeout = 10;

        services.AddHttpClient<ICoreApiClient, CoreApiClient>(client =>
        {
            client.BaseAddress = coreBaseUrl;
            client.Timeout     = TimeSpan.FromSeconds(coreTimeout);
        });

        return services;
    }

    // Resolve a URL de um servico integrado. Fora de Development a URL tem de vir
    // da configuracao: um default de localhost dentro de um container significa
    // integracao que falha em runtime, e nao no boot, onde da para ver.
    private static Uri UrlDoServico(IConfiguration cfg, IHostEnvironment env, string chave, string urlDeDesenvolvimento)
    {
        var valor = cfg[chave];
        if (!string.IsNullOrWhiteSpace(valor))
            return new Uri(valor);

        if (env.IsDevelopment())
            return new Uri(urlDeDesenvolvimento);

        throw new InvalidOperationException(
            $"Configuracao obrigatoria ausente: '{chave}'. " +
            $"Defina a variavel de ambiente '{chave.Replace(":", "__")}' com a URL do servico.");
    }
}
