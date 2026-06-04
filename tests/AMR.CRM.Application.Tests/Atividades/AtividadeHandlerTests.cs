using AMR.CRM.Application.Atividades.Commands;
using AMR.CRM.Application.Atividades.Queries;
using AMR.CRM.Application.Tests.Leads;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Tests.Atividades;

public class AtividadeHandlerTests
{
    private static (FakeAtividadeRepository repo, FakeUnitOfWork uow) Fakes() =>
        (new FakeAtividadeRepository(), new FakeUnitOfWork());

    private static Atividade AtividadeNova(FakeAtividadeRepository repo)
    {
        var a = Atividade.Criar(Guid.NewGuid(), TipoAtividade.Ligacao,
            "Ligar para o cliente", DateTime.UtcNow.AddHours(-1));
        repo.Store.Add(a);
        return a;
    }

    // ── Criar ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Criar_DeveCriarAtividadeNaoConcluidaPorPadrao()
    {
        var a = Atividade.Criar(Guid.NewGuid(), TipoAtividade.Tarefa, "Enviar proposta", DateTime.UtcNow);
        Assert.False(a.Concluida);
        Assert.Equal(TipoAtividade.Tarefa, a.Tipo);
    }

    [Fact]
    public void Criar_ComDescricaoVazia_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Atividade.Criar(Guid.NewGuid(), TipoAtividade.Email, "  ", DateTime.UtcNow));
    }

    // ── Concluir ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task ConcluirHandler_DeveMudarConcluida()
    {
        var (repo, uow) = Fakes();
        var a = AtividadeNova(repo);

        var handler = new ConcluirAtividadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new ConcluirAtividadeCommand(a.Id), default);

        Assert.True(result.Sucesso);
        Assert.True(a.Concluida);
    }

    [Fact]
    public async Task ConcluirHandler_QuandoChamadoDuasVezes_DeveVoltarParaPendente()
    {
        var (repo, uow) = Fakes();
        var a = AtividadeNova(repo);

        var handler = new ConcluirAtividadeCommandHandler(repo, uow);
        await handler.Handle(new ConcluirAtividadeCommand(a.Id), default);
        await handler.Handle(new ConcluirAtividadeCommand(a.Id), default);

        Assert.False(a.Concluida);
    }

    [Fact]
    public async Task ConcluirHandler_QuandoNaoEncontrada_DeveRetornarFalha()
    {
        var (repo, uow) = Fakes();
        var handler = new ConcluirAtividadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new ConcluirAtividadeCommand(Guid.NewGuid()), default);

        Assert.False(result.Sucesso);
        Assert.Equal("Atividade não encontrada.", result.Erro);
    }

    // ── Excluir ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task ExcluirHandler_DeveRemoverAtividade()
    {
        var (repo, uow) = Fakes();
        var a = AtividadeNova(repo);

        var handler = new ExcluirAtividadeCommandHandler(repo, uow);
        var result  = await handler.Handle(new ExcluirAtividadeCommand(a.Id), default);

        Assert.True(result.Sucesso);
        Assert.Empty(repo.Store);
    }

    // ── Listar ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ListarHandler_DeveRetornarApenasDaOportunidadeCorreta()
    {
        var (repo, _) = Fakes();
        var opId1 = Guid.NewGuid();
        var opId2 = Guid.NewGuid();

        repo.Store.Add(Atividade.Criar(opId1, TipoAtividade.Email, "A1", DateTime.UtcNow));
        repo.Store.Add(Atividade.Criar(opId1, TipoAtividade.Reuniao, "A2", DateTime.UtcNow.AddHours(-1)));
        repo.Store.Add(Atividade.Criar(opId2, TipoAtividade.Tarefa, "A3", DateTime.UtcNow));

        var handler = new ListarAtividadesQueryHandler(repo);
        var result  = await handler.Handle(new ListarAtividadesQuery(opId1), default);

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(opId1, a.OportunidadeId));
    }
}
