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
        await SeedContatosAsync(ctx);
        await SeedLeadsAsync(ctx);
        await SeedOportunidadesAsync(ctx);
        await SeedAtividadesAsync(ctx);
    }

    // ── Contatos ──────────────────────────────────────────────────────────────
    private static async Task SeedContatosAsync(AmrCrmDbContext ctx)
    {
        if (await ctx.Contatos.AnyAsync()) return;

        ctx.Contatos.AddRange(
            Contato.Criar("Carlos Eduardo Martins",  "carlos.martins@metalsp.com.br",
                TipoContato.Cliente,  "(11) 99100-0001", "Metalúrgica São Paulo Ltda"),
            Contato.Criar("Ana Paula Ferreira",       "ana.ferreira@nortesul.com.br",
                TipoContato.Prospect, "(21) 99200-0002", "Distribuidora Norte Sul S.A."),
            Contato.Criar("Roberto Silva",            "roberto.silva@construtoraalfa.com.br",
                TipoContato.Cliente,  "(31) 99300-0003", "Construtora Alfa Engenharia"),
            Contato.Criar("Fernanda Lima",            "fernanda.lima@techsul.com.br",
                TipoContato.Prospect, "(11) 99400-0004", "TechSul Automação"),
            Contato.Criar("Paulo Mendes",             "paulo.mendes@logexpress.com.br",
                TipoContato.Parceiro, "(11) 99500-0005", "Logística Expressa")
        );
        await ctx.SaveChangesAsync();
    }

    // ── Leads ─────────────────────────────────────────────────────────────────
    private static async Task SeedLeadsAsync(AmrCrmDbContext ctx)
    {
        if (await ctx.Leads.AnyAsync()) return;

        var leads = new[]
        {
            Lead.Criar("Marcos Oliveira",    "marcos.oliveira@industrias-beta.com.br",
                OrigemLead.LinkedIn,    "(11) 98700-1001", "Indústrias Beta Ltda",
                valorEstimado: 120_000m),
            Lead.Criar("Juliana Costa",      "juliana.costa@agro-max.com.br",
                OrigemLead.Website,     "(19) 98700-2002", "AgroMax Soluções",
                valorEstimado: 45_000m),
            Lead.Criar("Ricardo Fernandes",  "ricardo.fernandes@construflex.com.br",
                OrigemLead.Indicacao,   "(31) 98700-3003", "ConstruFlex Engenharia",
                valorEstimado: 280_000m),
            Lead.Criar("Patrícia Rocha",     "patricia.rocha@softprime.com.br",
                OrigemLead.Evento,      "(11) 98700-4004", "SoftPrime Tecnologia",
                valorEstimado: 60_000m),
            Lead.Criar("Eduardo Braga",      "eduardo.braga@delta-comercial.com.br",
                OrigemLead.Email,       "(21) 98700-5005", "Delta Comercial",
                valorEstimado: 18_000m),
        };

        leads[1].AvancarStatus(StatusLead.Qualificado);

        leads[2].AvancarStatus(StatusLead.Qualificado);
        leads[2].AvancarStatus(StatusLead.Proposta);

        leads[3].AvancarStatus(StatusLead.Qualificado);
        leads[3].AvancarStatus(StatusLead.Proposta);
        leads[3].AvancarStatus(StatusLead.Ganho);

        leads[4].AvancarStatus(StatusLead.Perdido);

        ctx.Leads.AddRange(leads);
        await ctx.SaveChangesAsync();
    }

    // ── Oportunidades ─────────────────────────────────────────────────────────
    private static async Task SeedOportunidadesAsync(AmrCrmDbContext ctx)
    {
        if (await ctx.Oportunidades.AnyAsync()) return;

        var leads    = await ctx.Leads.ToListAsync();
        var contatos = await ctx.Contatos.ToListAsync();

        var op1 = Oportunidade.Criar(
            "Contrato de fornecimento anual", 120_000m, 30,
            leadId: leads[0].Id,
            descricao: "Proposta enviada. Aguarda aprovação da diretoria.",
            previsaoFechamento: DateTime.UtcNow.AddDays(45),
            etapa: EtapaOportunidade.Proposta);

        var op2 = Oportunidade.Criar(
            "Automação linha de produção", 280_000m, 65,
            leadId: leads[2].Id,
            descricao: "Proposta técnica apresentada. Em negociação de escopo.",
            previsaoFechamento: DateTime.UtcNow.AddDays(30),
            etapa: EtapaOportunidade.Negociacao);
        op2.IniciarAndamento();

        var op3 = Oportunidade.Criar(
            "Pacote software MES — fase 1", 60_000m, 90,
            leadId: leads[3].Id,
            descricao: "Lead ganho. Oportunidade de expansão — fase 2 planejada.",
            previsaoFechamento: DateTime.UtcNow.AddDays(15),
            etapa: EtapaOportunidade.Fechamento);
        op3.Ganhar();

        var op4 = Oportunidade.Criar(
            "Serviços de manutenção preventiva", 36_000m, 50,
            contatoId: contatos[0].Id,
            descricao: "Contrato recorrente com cliente existente.",
            previsaoFechamento: DateTime.UtcNow.AddDays(60),
            etapa: EtapaOportunidade.Qualificacao);

        var op5 = Oportunidade.Criar(
            "Implantação módulo fiscal", 75_000m, 25,
            leadId: leads[1].Id,
            descricao: "Primeiro contato. Necessidade identificada.",
            previsaoFechamento: DateTime.UtcNow.AddDays(90),
            etapa: EtapaOportunidade.Prospeccao);

        var op6 = Oportunidade.Criar(
            "Upgrade ERP integrado", 180_000m, 70,
            contatoId: contatos[2].Id,
            descricao: "Reunião técnica realizada. Proposta sob análise.",
            previsaoFechamento: DateTime.UtcNow.AddDays(20),
            etapa: EtapaOportunidade.Negociacao);
        op6.IniciarAndamento();

        ctx.Oportunidades.AddRange(op1, op2, op3, op4, op5, op6);
        await ctx.SaveChangesAsync();
    }

    // ── Atividades ────────────────────────────────────────────────────────────
    private static async Task SeedAtividadesAsync(AmrCrmDbContext ctx)
    {
        if (await ctx.Atividades.AnyAsync()) return;

        var leads         = await ctx.Leads.ToListAsync();
        var oportunidades = await ctx.Oportunidades.ToListAsync();

        var a1 = Atividade.Criar(
            "Ligar para Marcos sobre proposta", TipoAtividade.Ligacao,
            DateTime.UtcNow.AddDays(3),
            leadId: leads[0].Id,
            observacao: "Confirmar interesse após envio da proposta inicial.");

        var a2 = Atividade.Criar(
            "Enviar e-mail de follow-up — AgroMax", TipoAtividade.Email,
            DateTime.UtcNow.AddDays(-5),
            leadId: leads[1].Id,
            observacao: "Apresentar cases de sucesso do setor agrícola.");
        a2.Atualizar(a2.Titulo, a2.Tipo, a2.DataPrevista, StatusAtividade.Concluida, a2.Observacao);

        var a3 = Atividade.Criar(
            "Reunião de alinhamento técnico — Contrato anual", TipoAtividade.Reuniao,
            DateTime.UtcNow.AddDays(-2),
            oportunidadeId: oportunidades[0].Id,
            observacao: "Definir escopo técnico com equipe de TI do cliente.");

        var a4 = Atividade.Criar(
            "Preparar proposta comercial — ConstruFlex", TipoAtividade.Tarefa,
            DateTime.UtcNow.AddDays(7),
            leadId: leads[2].Id,
            observacao: "Incluir cronograma detalhado e condições de pagamento.");
        a4.Atualizar(a4.Titulo, a4.Tipo, a4.DataPrevista, StatusAtividade.Cancelada, a4.Observacao);

        var a5 = Atividade.Criar(
            "Visita técnica — Automação linha de produção", TipoAtividade.Visita,
            DateTime.UtcNow.AddDays(-1),
            oportunidadeId: oportunidades[1].Id,
            observacao: "Levantamento de requisitos in loco na planta industrial.");

        var a6 = Atividade.Criar(
            "Agendar demo do módulo MES — SoftPrime", TipoAtividade.Ligacao,
            DateTime.UtcNow.AddDays(14),
            leadId: leads[3].Id,
            observacao: "Demonstração para o time técnico e financeiro.");

        ctx.Atividades.AddRange(a1, a2, a3, a4, a5, a6);
        await ctx.SaveChangesAsync();
    }
}
