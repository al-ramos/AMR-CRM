using AMR.CRM.Application.Leads.Commands;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Tests.Leads;

public class LeadHandlerTests
{
    private static (FakeLeadRepository repo, FakeUnitOfWork uow) Fakes() =>
        (new FakeLeadRepository(), new FakeUnitOfWork());

    private static Lead LeadNovo(FakeLeadRepository repo)
    {
        var lead = Lead.Criar("Ana Costa", "ana@exemplo.com", OrigemLead.LinkedIn, 10_000);
        repo.Store.Add(lead);
        return lead;
    }

    // ── Qualificar ────────────────────────────────────────────────────────────

    [Fact]
    public async Task QualificarHandler_QuandoLeadNovo_DeveRetornarSucesso()
    {
        var (repo, uow) = Fakes();
        var lead = LeadNovo(repo);

        var handler = new QualificarLeadCommandHandler(repo, uow);
        var result  = await handler.Handle(new QualificarLeadCommand(lead.Id), default);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusLead.Qualificado, lead.Status);
    }

    [Fact]
    public async Task QualificarHandler_QuandoNaoEncontrado_DeveRetornarFalha()
    {
        var (repo, uow) = Fakes();
        var handler = new QualificarLeadCommandHandler(repo, uow);
        var result  = await handler.Handle(new QualificarLeadCommand(Guid.NewGuid()), default);

        Assert.False(result.Sucesso);
        Assert.Equal("Lead não encontrado.", result.Erro);
    }

    // ── Ganhar ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GanharHandler_QuandoQualificado_DeveRetornarSucesso()
    {
        var (repo, uow) = Fakes();
        var lead = LeadNovo(repo);
        lead.Qualificar();

        var handler = new GanharLeadCommandHandler(repo, uow);
        var result  = await handler.Handle(new GanharLeadCommand(lead.Id), default);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusLead.Ganho, lead.Status);
    }

    [Fact]
    public async Task GanharHandler_QuandoJaEncerrado_DeveLancarExcecao()
    {
        var (repo, uow) = Fakes();
        var lead = LeadNovo(repo);
        lead.Qualificar();
        lead.Perder();

        var handler = new GanharLeadCommandHandler(repo, uow);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new GanharLeadCommand(lead.Id), default));
    }

    // ── Perder ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PerderHandler_QuandoQualificado_DeveRetornarSucesso()
    {
        var (repo, uow) = Fakes();
        var lead = LeadNovo(repo);
        lead.Qualificar();

        var handler = new PerderLeadCommandHandler(repo, uow);
        var result  = await handler.Handle(new PerderLeadCommand(lead.Id), default);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusLead.Perdido, lead.Status);
    }

    [Fact]
    public async Task PerderHandler_QuandoJaGanho_DeveLancarExcecao()
    {
        var (repo, uow) = Fakes();
        var lead = LeadNovo(repo);
        lead.Qualificar();
        lead.Ganhar();

        var handler = new PerderLeadCommandHandler(repo, uow);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new PerderLeadCommand(lead.Id), default));
    }

    // ── Excluir ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task ExcluirHandler_DeveRemoverLead()
    {
        var (repo, uow) = Fakes();
        var lead = LeadNovo(repo);

        var handler = new ExcluirLeadCommandHandler(repo, uow);
        var result  = await handler.Handle(new ExcluirLeadCommand(lead.Id), default);

        Assert.True(result.Sucesso);
        Assert.Empty(repo.Store);
    }
}
