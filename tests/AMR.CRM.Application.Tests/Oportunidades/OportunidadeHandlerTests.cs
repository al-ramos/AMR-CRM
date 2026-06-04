using AMR.CRM.Application.Oportunidades.Commands;
using AMR.CRM.Application.Tests.Leads;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Tests.Oportunidades;

public class OportunidadeHandlerTests
{
    private static (FakeOportunidadeRepository repo, FakeUnitOfWork uow) Fakes() =>
        (new FakeOportunidadeRepository(), new FakeUnitOfWork());

    private static Oportunidade OportunidadeAberta(FakeOportunidadeRepository repo)
    {
        var op = Oportunidade.Criar(null, "Oportunidade Teste", 50_000, leadId: Guid.NewGuid());
        repo.Store.Add(op);
        return op;
    }

    // ── Ganhar ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GanharHandler_QuandoAberta_DeveRetornarSucesso()
    {
        var (repo, uow) = Fakes();
        var op = OportunidadeAberta(repo);

        var handler = new AvancarOportunidadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new AvancarOportunidadeCommand(op.Id, StatusOportunidade.Ganha), default);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusOportunidade.Ganha, op.Status);
    }

    [Fact]
    public async Task GanharHandler_QuandoEmAndamento_DeveRetornarSucesso()
    {
        var (repo, uow) = Fakes();
        var op = OportunidadeAberta(repo);
        op.IniciarAndamento();

        var handler = new AvancarOportunidadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new AvancarOportunidadeCommand(op.Id, StatusOportunidade.Ganha), default);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusOportunidade.Ganha, op.Status);
    }

    [Fact]
    public async Task GanharHandler_QuandoJaCancelada_DeveLancarExcecao()
    {
        var (repo, uow) = Fakes();
        var op = OportunidadeAberta(repo);
        op.Cancelar();

        var handler = new AvancarOportunidadeCommandHandler(repo, uow);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new AvancarOportunidadeCommand(op.Id, StatusOportunidade.Ganha), default));
    }

    [Fact]
    public async Task GanharHandler_QuandoNaoEncontrado_DeveRetornarFalha()
    {
        var (repo, uow) = Fakes();
        var handler = new AvancarOportunidadeCommandHandler(repo, uow);
        var result  = await handler.Handle(
            new AvancarOportunidadeCommand(Guid.NewGuid(), StatusOportunidade.Ganha), default);

        Assert.False(result.Sucesso);
        Assert.Equal("Oportunidade não encontrada.", result.Erro);
    }

    // ── Perder ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task PerderHandler_QuandoAberta_DeveRetornarSucesso()
    {
        var (repo, uow) = Fakes();
        var op = OportunidadeAberta(repo);

        var handler = new AvancarOportunidadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new AvancarOportunidadeCommand(op.Id, StatusOportunidade.Perdida), default);

        Assert.True(result.Sucesso);
        Assert.Equal(StatusOportunidade.Perdida, op.Status);
    }

    [Fact]
    public async Task PerderHandler_QuandoJaGanha_DeveLancarExcecao()
    {
        var (repo, uow) = Fakes();
        var op = OportunidadeAberta(repo);
        op.Ganhar();

        var handler = new AvancarOportunidadeCommandHandler(repo, uow);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(new AvancarOportunidadeCommand(op.Id, StatusOportunidade.Perdida), default));
    }

    // ── Excluir ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task ExcluirHandler_DeveRemoverOportunidade()
    {
        var (repo, uow) = Fakes();
        var op = OportunidadeAberta(repo);

        var handler = new ExcluirOportunidadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new ExcluirOportunidadeCommand(op.Id), default);

        Assert.True(result.Sucesso);
        Assert.Empty(repo.Store);
    }
}
