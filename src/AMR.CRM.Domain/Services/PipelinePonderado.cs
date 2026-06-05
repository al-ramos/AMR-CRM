namespace AMR.CRM.Domain.Services;

public static class PipelinePonderado
{
    public static decimal Calcular(IEnumerable<(decimal Valor, int Probabilidade)> oportunidades)
        => oportunidades.Sum(o => o.Valor * o.Probabilidade / 100m);
}
