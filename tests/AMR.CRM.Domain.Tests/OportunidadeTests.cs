using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Tests;

public class OportunidadeTests
{
    private static readonly Guid ContatoId = Guid.NewGuid();

    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarOportunidadeAberta()
    {
        var op = Oportunidade.Criar("Contrato anual", 50000m, contatoId: ContatoId);

        Assert.Equal(ContatoId, op.ContatoId);
        Assert.Equal("Contrato anual", op.Titulo);
        Assert.Equal(50000m, op.Valor);
        Assert.Equal(50, op.Probabilidade);   // default
        Assert.Equal(StatusOportunidade.Aberta, op.Status);
    }

    [Fact]
    public void Criar_ComProbabilidade_DeveArmazenar()
    {
        var op = Oportunidade.Criar("Negociação", 10000m, 70, contatoId: ContatoId);
        Assert.Equal(70, op.Probabilidade);
    }

    [Fact]
    public void Criar_SemContatoOuLead_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Oportunidade.Criar("Título", 1000m));
    }

    [Fact]
    public void IniciarAndamento_DeveAlterarStatus()
    {
        var op = Oportunidade.Criar("Negociação", 10000m, contatoId: ContatoId);
        op.IniciarAndamento();
        Assert.Equal(StatusOportunidade.EmAndamento, op.Status);
    }

    [Fact]
    public void Ganhar_DeveAlterarStatusParaGanha()
    {
        var op = Oportunidade.Criar("Deal fechado", 25000m, contatoId: ContatoId);
        op.Ganhar();
        Assert.Equal(StatusOportunidade.Ganha, op.Status);
    }

    [Fact]
    public void Cancelar_AposGanha_DeveLancarExcecao()
    {
        var op = Oportunidade.Criar("Oportunidade", 1000m, contatoId: ContatoId);
        op.Ganhar();
        Assert.Throws<InvalidOperationException>(() => op.Cancelar());
    }

    [Fact]
    public void Criar_ValorNegativo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Oportunidade.Criar("Oportunidade", -100m, contatoId: ContatoId));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Criar_ProbabilidadeForaDeLimite_DeveLancarExcecao(int probabilidade)
    {
        Assert.Throws<ArgumentException>(() =>
            Oportunidade.Criar("Oportunidade", 1000m, probabilidade, contatoId: ContatoId));
    }
}
