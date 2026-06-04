using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Tests;

public class LeadTests
{
    private static Lead CriarLead() =>
        Lead.Criar("João Silva", "joao@exemplo.com", OrigemLead.Site, 5000);

    [Fact]
    public void Criar_DeveCriarLeadComStatusNovo()
    {
        var lead = CriarLead();
        Assert.Equal(StatusLead.Novo, lead.Status);
        Assert.Equal("joao@exemplo.com", lead.Email);
    }

    [Fact]
    public void Qualificar_DeveTransicionarParaQualificado()
    {
        var lead = CriarLead();
        lead.Qualificar();
        Assert.Equal(StatusLead.Qualificado, lead.Status);
    }

    [Fact]
    public void Qualificar_QuandoNaoNovo_DeveLancarExcecao()
    {
        var lead = CriarLead();
        lead.Qualificar();
        Assert.Throws<InvalidOperationException>(() => lead.Qualificar());
    }

    [Fact]
    public void EnviarProposta_DeveTransicionarParaProposta()
    {
        var lead = CriarLead();
        lead.Qualificar();
        lead.EnviarProposta();
        Assert.Equal(StatusLead.Proposta, lead.Status);
    }

    [Fact]
    public void EnviarProposta_QuandoNaoQualificado_DeveLancarExcecao()
    {
        var lead = CriarLead();
        Assert.Throws<InvalidOperationException>(() => lead.EnviarProposta());
    }

    [Fact]
    public void Ganhar_DeveTransicionarParaGanho()
    {
        var lead = CriarLead();
        lead.Qualificar();
        lead.Ganhar();
        Assert.Equal(StatusLead.Ganho, lead.Status);
    }

    [Fact]
    public void Ganhar_QuandoJaEncerrado_DeveLancarExcecao()
    {
        var lead = CriarLead();
        lead.Qualificar();
        lead.Ganhar();
        Assert.Throws<InvalidOperationException>(() => lead.Ganhar());
    }

    [Fact]
    public void Perder_DeveTransicionarParaPerdido()
    {
        var lead = CriarLead();
        lead.Qualificar();
        lead.Perder();
        Assert.Equal(StatusLead.Perdido, lead.Status);
    }

    [Fact]
    public void Perder_QuandoJaEncerrado_DeveLancarExcecao()
    {
        var lead = CriarLead();
        lead.Qualificar();
        lead.Perder();
        Assert.Throws<InvalidOperationException>(() => lead.Perder());
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Lead.Criar("João", "joao@exemplo.com", OrigemLead.Site, -100));
    }
}
