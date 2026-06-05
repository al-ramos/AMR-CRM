using AMR.CRM.Domain.Services;

namespace AMR.CRM.Domain.Tests;

public class PipelinePonderadoTests
{
    [Fact]
    public void PipelinePonderado_DeveCalcularCorretamente()
    {
        // Arrange
        var oportunidades = new[]
        {
            (Valor: 100_000m, Probabilidade: 50),  // → 50_000
            (Valor: 200_000m, Probabilidade: 25),  // → 50_000
            (Valor:  50_000m, Probabilidade: 80),  // → 40_000
        };

        // Act
        var resultado = PipelinePonderado.Calcular(oportunidades);

        // Assert
        Assert.Equal(140_000m, resultado);
    }

    [Fact]
    public void PipelinePonderado_SemOportunidades_DeveRetornarZero()
    {
        var resultado = PipelinePonderado.Calcular([]);

        Assert.Equal(0m, resultado);
    }

    [Fact]
    public void PipelinePonderado_ComProbabilidade100_DeveRetornarValorIntegral()
    {
        var resultado = PipelinePonderado.Calcular([(Valor: 75_000m, Probabilidade: 100)]);

        Assert.Equal(75_000m, resultado);
    }

    [Fact]
    public void PipelinePonderado_ComProbabilidadeZero_DeveRetornarZero()
    {
        var resultado = PipelinePonderado.Calcular([(Valor: 500_000m, Probabilidade: 0)]);

        Assert.Equal(0m, resultado);
    }
}
