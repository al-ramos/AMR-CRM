using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Oportunidade
{
    public Guid               Id            { get; private set; }
    public Guid?              ContatoId     { get; private set; }
    public Guid?              LeadId        { get; private set; }
    public string             Titulo        { get; private set; } = default!;
    public decimal            Valor         { get; private set; }
    public decimal            Probabilidade { get; private set; }
    public StatusOportunidade Status        { get; private set; }
    public string?            Descricao     { get; private set; }
    public DateTime?          PrevisaoFechamento { get; private set; }
    public DateTime           CriadoEm     { get; private set; }
    public DateTime           AlteradoEm   { get; private set; }

    public Contato? Contato { get; private set; }
    public Lead?    Lead    { get; private set; }

    private Oportunidade() { }

    public static Oportunidade Criar(Guid? contatoId, string titulo, decimal valor,
        string? descricao = null, DateTime? previsaoFechamento = null,
        Guid? leadId = null, decimal probabilidade = 0)
    {
        if (contatoId is null && leadId is null)
            throw new ArgumentException("É obrigatório informar o Contato ou Lead.");
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        if (valor < 0) throw new ArgumentException("Valor não pode ser negativo.", nameof(valor));
        if (probabilidade is < 0 or > 100) throw new ArgumentException("Probabilidade deve estar entre 0 e 100.", nameof(probabilidade));

        return new Oportunidade
        {
            Id                 = Guid.NewGuid(),
            ContatoId          = contatoId,
            LeadId             = leadId,
            Titulo             = titulo.Trim(),
            Valor              = valor,
            Probabilidade      = probabilidade,
            Status             = StatusOportunidade.Aberta,
            Descricao          = descricao?.Trim(),
            PrevisaoFechamento = previsaoFechamento,
            CriadoEm          = DateTime.UtcNow,
            AlteradoEm         = DateTime.UtcNow
        };
    }

    public void Atualizar(string titulo, decimal valor, decimal probabilidade = 0,
        string? descricao = null, DateTime? previsaoFechamento = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        if (valor < 0) throw new ArgumentException("Valor não pode ser negativo.", nameof(valor));
        if (probabilidade is < 0 or > 100) throw new ArgumentException("Probabilidade deve estar entre 0 e 100.", nameof(probabilidade));

        Titulo             = titulo.Trim();
        Valor              = valor;
        Probabilidade      = probabilidade;
        Descricao          = descricao?.Trim();
        PrevisaoFechamento = previsaoFechamento;
        AlteradoEm         = DateTime.UtcNow;
    }

    public void IniciarAndamento()
    {
        if (Status != StatusOportunidade.Aberta)
            throw new InvalidOperationException("Apenas oportunidades Abertas podem ser iniciadas.");
        Status     = StatusOportunidade.EmAndamento;
        AlteradoEm = DateTime.UtcNow;
    }

    public void Ganhar()
    {
        if (Status is StatusOportunidade.Cancelada or StatusOportunidade.Perdida)
            throw new InvalidOperationException("Oportunidade já encerrada.");
        Status     = StatusOportunidade.Ganha;
        AlteradoEm = DateTime.UtcNow;
    }

    public void Perder()
    {
        if (Status is StatusOportunidade.Cancelada or StatusOportunidade.Ganha)
            throw new InvalidOperationException("Oportunidade já encerrada.");
        Status     = StatusOportunidade.Perdida;
        AlteradoEm = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        if (Status is StatusOportunidade.Ganha or StatusOportunidade.Perdida)
            throw new InvalidOperationException("Oportunidade já encerrada.");
        Status     = StatusOportunidade.Cancelada;
        AlteradoEm = DateTime.UtcNow;
    }
}
