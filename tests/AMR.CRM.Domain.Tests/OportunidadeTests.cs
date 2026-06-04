using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Tests;

public class OportunidadeTests
{
    private static readonly Guid ContatoId = Guid.NewGuid();

    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarOportunidadeAberta()
    {
        var op = Oportunidade.Criar(ContatoId, "Contrato anual", 50000m);

        Assert.Equal(ContatoId, op.ContatoId);
        Assert.Equal("Contrato anual", op.Titulo);
        Assert.Equal(50000m, op.Valor);
        Assert.Equal(StatusOportunidade.Aberta, op.Status);
    }

    [Fact]
    public void IniciarAndamento_DeveAlterarStatusParaEmAndamento()
    {
        var op = Oportunidade.Criar(ContatoId, "Negociação", 10000m);
        op.IniciarAndamento();
        Assert.Equal(StatusOportunidade.EmAndamento, op.Status);
    }

    [Fact]
    public void Ganhar_DeveAlterarStatusParaGanha()
    {
        var op = Oportunidade.Criar(ContatoId, "Deal fechado", 25000m);
        op.Ganhar();
        Assert.Equal(StatusOportunidade.Ganha, op.Status);
    }

    [Fact]
    public void Cancelar_AposGanha_DeveLancarExcecao()
    {
        var op = Oportunidade.Criar(ContatoId, "Oportunidade", 1000m);
        op.Ganhar();
        Assert.Throws<InvalidOperationException>(() => op.Cancelar());
    }

    [Fact]
    public void Criar_ValorNegativo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            Oportunidade.Criar(ContatoId, "Oportunidade", -100m));
    }
}
