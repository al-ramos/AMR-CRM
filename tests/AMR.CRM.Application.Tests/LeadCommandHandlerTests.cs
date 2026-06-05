using AMR.CRM.Application.Interfaces;
using AMR.CRM.Application.Leads.Commands;
using AMR.CRM.Application.Leads.Queries;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AMR.CRM.Application.Tests;

public class LeadCommandHandlerTests
{
    private readonly ILeadRepository _repo  = Substitute.For<ILeadRepository>();
    private readonly IUnitOfWork     _uow   = Substitute.For<IUnitOfWork>();

    // ── CriarLead ────────────────────────────────────────────────────────────
    [Fact]
    public async Task CriarLead_ComDadosValidos_DeveRetornarSucesso()
    {
        var cmd = new CriarLeadCommand("Empresa Teste", "teste@empresa.com", OrigemLead.Website,
            ValorEstimado: 10_000m);

        var handler = new CriarLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Sucesso);
        Assert.NotNull(result.Valor);
        Assert.Equal("Empresa Teste", result.Valor.Nome);
        Assert.Equal(StatusLead.Novo, result.Valor.Status);

        await _repo.Received(1).AdicionarAsync(Arg.Any<Lead>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ── AtualizarLead ────────────────────────────────────────────────────────
    [Fact]
    public async Task AtualizarLead_LeadExistente_DeveRetornarSucesso()
    {
        var lead = Lead.Criar("Nome Antigo", "old@t.com", OrigemLead.Manual, valorEstimado: 1000m);
        _repo.ObterPorIdAsync(lead.Id, Arg.Any<CancellationToken>()).Returns(lead);

        var cmd     = new AtualizarLeadCommand(lead.Id, "Nome Novo", "new@t.com", OrigemLead.LinkedIn, ValorEstimado: 5000m);
        var handler = new AtualizarLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Sucesso);
        _repo.Received(1).Atualizar(lead);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AtualizarLead_NaoEncontrado_DeveRetornarFalha()
    {
        _repo.ObterPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var cmd     = new AtualizarLeadCommand(Guid.NewGuid(), "N", "n@t.com", OrigemLead.Manual);
        var handler = new AtualizarLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.Sucesso);
        Assert.Contains("não encontrado", result.Erro, StringComparison.OrdinalIgnoreCase);
    }

    // ── AvancarStatus ────────────────────────────────────────────────────────
    [Fact]
    public async Task AvancarStatus_TransicaoValida_DeveAlterarStatus()
    {
        var lead = Lead.Criar("Lead X", "x@t.com", OrigemLead.Evento);
        _repo.ObterPorIdAsync(lead.Id, Arg.Any<CancellationToken>()).Returns(lead);

        var cmd     = new AvancarStatusLeadCommand(lead.Id, StatusLead.Qualificado);
        var handler = new AvancarStatusLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusLead.Qualificado, lead.Status);
    }

    [Fact]
    public async Task AvancarStatus_LeadNaoEncontrado_DeveRetornarFalha()
    {
        _repo.ObterPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var cmd     = new AvancarStatusLeadCommand(Guid.NewGuid(), StatusLead.Qualificado);
        var handler = new AvancarStatusLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.Sucesso);
    }

    // ── ExcluirLead ──────────────────────────────────────────────────────────
    [Fact]
    public async Task ExcluirLead_SemOportunidades_DeveRemover()
    {
        var lead = Lead.Criar("Lead Y", "y@t.com", OrigemLead.Manual);
        _repo.ObterPorIdAsync(lead.Id, Arg.Any<CancellationToken>()).Returns(lead);

        var cmd     = new ExcluirLeadCommand(lead.Id);
        var handler = new ExcluirLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Sucesso);
        _repo.Received(1).Remover(lead);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExcluirLead_NaoEncontrado_DeveRetornarFalha()
    {
        _repo.ObterPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var cmd     = new ExcluirLeadCommand(Guid.NewGuid());
        var handler = new ExcluirLeadCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.Sucesso);
    }

    // ── ListarLeads ──────────────────────────────────────────────────────────
    [Fact]
    public async Task ListarLeads_DeveRetornarLista()
    {
        var leads = new List<Lead>
        {
            Lead.Criar("Lead A", "a@t.com", OrigemLead.Website),
            Lead.Criar("Lead B", "b@t.com", OrigemLead.LinkedIn),
        };
        _repo.ListarAsync(Arg.Any<CancellationToken>()).Returns(leads);

        var handler = new ListarLeadsQueryHandler(_repo);
        var result  = await handler.Handle(new ListarLeadsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Nome == "Lead A");
    }
}
