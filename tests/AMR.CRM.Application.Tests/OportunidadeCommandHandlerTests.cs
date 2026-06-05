using AMR.CRM.Application.Interfaces;
using AMR.CRM.Application.Oportunidades.Commands;
using AMR.CRM.Application.Oportunidades.Queries;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AMR.CRM.Application.Tests;

public class OportunidadeCommandHandlerTests
{
    private readonly IOportunidadeRepository _repo        = Substitute.For<IOportunidadeRepository>();
    private readonly IContatoRepository      _contatoRepo = Substitute.For<IContatoRepository>();
    private readonly ILeadRepository         _leadRepo    = Substitute.For<ILeadRepository>();
    private readonly IUnitOfWork             _uow         = Substitute.For<IUnitOfWork>();

    // ── CriarOportunidade ────────────────────────────────────────────────────
    [Fact]
    public async Task CriarOportunidade_ComLeadValido_DeveRetornarSucesso()
    {
        var lead = Lead.Criar("Lead Teste", "lead@t.com", OrigemLead.Website, valorEstimado: 50_000m);
        _leadRepo.ObterPorIdAsync(lead.Id, Arg.Any<CancellationToken>()).Returns(lead);

        var cmd     = new CriarOportunidadeCommand("Contrato Anual", 100_000m, 70, LeadId: lead.Id);
        var handler = new CriarOportunidadeCommandHandler(_repo, _contatoRepo, _leadRepo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Sucesso);
        Assert.NotNull(result.Valor);
        Assert.Equal("Contrato Anual", result.Valor.Titulo);
        await _repo.Received(1).AdicionarAsync(Arg.Any<Oportunidade>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CriarOportunidade_LeadNaoExistente_DeveRetornarFalha()
    {
        _leadRepo.ObterPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var cmd     = new CriarOportunidadeCommand("Op X", 10_000m, LeadId: Guid.NewGuid());
        var handler = new CriarOportunidadeCommandHandler(_repo, _contatoRepo, _leadRepo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result.Sucesso);
        Assert.Contains("não encontrado", result.Erro, StringComparison.OrdinalIgnoreCase);
    }

    // ── AtualizarOportunidade ────────────────────────────────────────────────
    [Fact]
    public async Task AtualizarOportunidade_Existente_DeveRetornarSucesso()
    {
        var leadId = Guid.NewGuid();
        var op     = Oportunidade.Criar("Titulo Antigo", 10_000m, 50, leadId: leadId);
        _repo.ObterPorIdAsync(op.Id, Arg.Any<CancellationToken>()).Returns(op);

        var cmd = new AtualizarOportunidadeCommand(
            op.Id, "Titulo Novo", 20_000m, 80, EtapaOportunidade.Negociacao);
        var handler = new AtualizarOportunidadeCommandHandler(_repo, _uow);
        var result  = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Sucesso);
        Assert.Equal("Titulo Novo", op.Titulo);
        Assert.Equal(20_000m, op.Valor);
        _repo.Received(1).Atualizar(op);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ── ListarOportunidades ──────────────────────────────────────────────────
    [Fact]
    public async Task ListarOportunidades_DeveRetornarLista()
    {
        var leadId = Guid.NewGuid();
        var lista  = new List<Oportunidade>
        {
            Oportunidade.Criar("Op A", 10_000m, leadId: leadId),
            Oportunidade.Criar("Op B", 20_000m, leadId: leadId),
        };
        _repo.ListarAsync(Arg.Any<CancellationToken>()).Returns(lista);

        var handler = new ListarOportunidadesQueryHandler(_repo);
        var result  = await handler.Handle(new ListarOportunidadesQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Titulo == "Op A");
    }
}
