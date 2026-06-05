using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Tests;

public class LeadTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarLeadComStatusNovo()
    {
        var lead = Lead.Criar("João Silva", "joao@empresa.com.br", OrigemLead.LinkedIn,
            empresa: "Empresa Teste", valorEstimado: 50_000m);

        Assert.Equal("João Silva", lead.Nome);
        Assert.Equal("joao@empresa.com.br", lead.Email);
        Assert.Equal(OrigemLead.LinkedIn, lead.Origem);
        Assert.Equal(StatusLead.Novo, lead.Status);
        Assert.Equal(50_000m, lead.ValorEstimado);
        Assert.NotEqual(Guid.Empty, lead.Id);
    }

    [Fact]
    public void Criar_EmailDeveSerNormalizado()
    {
        var lead = Lead.Criar("Maria", "MARIA@EMPRESA.COM", OrigemLead.Website);
        Assert.Equal("maria@empresa.com", lead.Email);
    }

    [Fact]
    public void AvancarStatus_DeNovoParaQualificado_DeveAlterar()
    {
        var lead = Lead.Criar("Teste", "t@t.com", OrigemLead.Manual);
        lead.AvancarStatus(StatusLead.Qualificado);
        Assert.Equal(StatusLead.Qualificado, lead.Status);
    }

    [Fact]
    public void AvancarStatus_DeQualificadoParaProposta_DeveAlterar()
    {
        var lead = Lead.Criar("Teste", "t@t.com", OrigemLead.Manual);
        lead.AvancarStatus(StatusLead.Qualificado);
        lead.AvancarStatus(StatusLead.Proposta);
        Assert.Equal(StatusLead.Proposta, lead.Status);
    }

    [Fact]
    public void AvancarStatus_ParaGanho_DeveAlterar()
    {
        var lead = Lead.Criar("Teste", "t@t.com", OrigemLead.Indicacao);
        lead.AvancarStatus(StatusLead.Qualificado);
        lead.AvancarStatus(StatusLead.Ganho);
        Assert.Equal(StatusLead.Ganho, lead.Status);
    }

    [Fact]
    public void AvancarStatus_AposGanho_DeveLancarExcecao()
    {
        var lead = Lead.Criar("Teste", "t@t.com", OrigemLead.Evento);
        lead.AvancarStatus(StatusLead.Ganho);
        Assert.Throws<InvalidOperationException>(() => lead.AvancarStatus(StatusLead.Proposta));
    }

    [Fact]
    public void AvancarStatus_AposPerdido_DeveLancarExcecao()
    {
        var lead = Lead.Criar("Teste", "t@t.com", OrigemLead.Email);
        lead.AvancarStatus(StatusLead.Perdido);
        Assert.Throws<InvalidOperationException>(() => lead.AvancarStatus(StatusLead.Qualificado));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_NomeVazio_DeveLancarExcecao(string nome)
    {
        Assert.Throws<ArgumentException>(() =>
            Lead.Criar(nome, "email@t.com", OrigemLead.Manual));
    }

    [Fact]
    public void Criar_ValorEstimadoNegativo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Lead.Criar("Teste", "t@t.com", OrigemLead.Manual, valorEstimado: -1m));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_EmailVazio_DeveLancarExcecao(string email)
    {
        Assert.Throws<ArgumentException>(() =>
            Lead.Criar("Nome Válido", email, OrigemLead.Manual));
    }

    [Fact]
    public void Atualizar_DeveAlterarCampos()
    {
        var lead = Lead.Criar("Nome Antigo", "antigo@t.com", OrigemLead.Manual,
            empresa: "Empresa A", valorEstimado: 1000m);

        lead.Atualizar("Nome Novo", "novo@t.com", OrigemLead.LinkedIn,
            empresa: "Empresa B", valorEstimado: 5000m);

        Assert.Equal("Nome Novo", lead.Nome);
        Assert.Equal("novo@t.com", lead.Email);
        Assert.Equal(OrigemLead.LinkedIn, lead.Origem);
        Assert.Equal("Empresa B", lead.Empresa);
        Assert.Equal(5000m, lead.ValorEstimado);
    }
}
