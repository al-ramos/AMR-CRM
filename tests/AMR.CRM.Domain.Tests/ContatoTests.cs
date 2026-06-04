using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Tests;

public class ContatoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarContatoAtivo()
    {
        var contato = Contato.Criar("João Silva", "joao@email.com", TipoContato.Lead);

        Assert.Equal("João Silva", contato.Nome);
        Assert.Equal("joao@email.com", contato.Email);
        Assert.Equal(TipoContato.Lead, contato.Tipo);
        Assert.Equal(StatusContato.Ativo, contato.Status);
        Assert.NotEqual(Guid.Empty, contato.Id);
    }

    [Fact]
    public void Criar_EmailDeveSerLowerCase()
    {
        var contato = Contato.Criar("Maria", "MARIA@EMAIL.COM", TipoContato.Prospect);
        Assert.Equal("maria@email.com", contato.Email);
    }

    [Fact]
    public void Inativar_DeveAlterarStatusParaInativo()
    {
        var contato = Contato.Criar("João", "joao@email.com", TipoContato.Lead);
        contato.Inativar();
        Assert.Equal(StatusContato.Inativo, contato.Status);
    }

    [Fact]
    public void Reativar_DeveAlterarStatusParaAtivo()
    {
        var contato = Contato.Criar("João", "joao@email.com", TipoContato.Lead);
        contato.Inativar();
        contato.Reativar();
        Assert.Equal(StatusContato.Ativo, contato.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_NomeVazio_DeveLancarExcecao(string nome)
    {
        Assert.Throws<ArgumentException>(() =>
            Contato.Criar(nome, "email@test.com", TipoContato.Lead));
    }
}
