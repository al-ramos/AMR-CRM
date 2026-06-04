using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AMR.CRM.Infrastructure.Data;

/// <summary>
/// Seed de dados demo para o AMR-CRM.
/// Idempotente — só insere se as tabelas estiverem vazias.
/// </summary>
public static class AmrCrmSeed
{
    public static async Task AplicarAsync(AmrCrmDbContext ctx)
    {
        if (await ctx.Contatos.AnyAsync()) return;

        var leads = new[]
        {
            Contato.Criar("Carlos Eduardo Martins", "carlos.martins@metalsp.com.br",
                TipoContato.Lead, "(11) 99100-0001", "Metalúrgica São Paulo Ltda"),
            Contato.Criar("Ana Paula Ferreira", "ana.ferreira@nortesul.com.br",
                TipoContato.Prospect, "(21) 99200-0002", "Distribuidora Norte Sul S.A."),
            Contato.Criar("Roberto Silva", "roberto.silva@construtoraalfa.com.br",
                TipoContato.Cliente, "(31) 99300-0003", "Construtora Alfa Engenharia"),
            Contato.Criar("Fernanda Lima", "fernanda.lima@techsul.com.br",
                TipoContato.Prospect, "(11) 99400-0004", "TechSul Automação"),
            Contato.Criar("Paulo Mendes", "paulo.mendes@logexpress.com.br",
                TipoContato.Parceiro, "(11) 99500-0005", "Logística Expressa"),
        };

        ctx.Contatos.AddRange(leads);
        await ctx.SaveChangesAsync();

        if (await ctx.Oportunidades.AnyAsync()) return;

        var oportunidades = new[]
        {
            Oportunidade.Criar(leads[0].Id, "Fornecimento de peças — contrato anual",
                85000m, "Proposta enviada, aguardando aprovação do comitê",
                DateTime.UtcNow.AddDays(30)),
            Oportunidade.Criar(leads[1].Id, "Equipamentos linha de montagem",
                120000m, "Reunião técnica agendada",
                DateTime.UtcNow.AddDays(45)),
            Oportunidade.Criar(leads[2].Id, "Manutenção preventiva — pacote anual",
                36000m, null,
                DateTime.UtcNow.AddDays(15)),
            Oportunidade.Criar(leads[3].Id, "Integração sistema MES",
                55000m, "Avaliação técnica em andamento",
                DateTime.UtcNow.AddDays(60)),
        };

        oportunidades[1].IniciarAndamento();
        oportunidades[2].Ganhar();

        ctx.Oportunidades.AddRange(oportunidades);
        await ctx.SaveChangesAsync();
    }
}
