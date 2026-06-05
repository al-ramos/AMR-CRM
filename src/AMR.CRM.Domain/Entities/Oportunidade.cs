using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Oportunidade
{
    public Guid               Id                 { get; private set; }
    public Guid?              ContatoId          { get; private set; }   // opcional (legado)
    public Guid?              LeadId             { get; private set; }   // FK para Lead
    public string             Titulo             { get; private set; } = default!;
    public decimal            Valor              { get; private set; }
    public int                Probabilidade      { get; private set; }   // 0–100 %
    public EtapaOportunidade  Etapa              { get; private set; }
    public StatusOportunidade Status             { get; private set; }
    public string?            Descricao          { get; private set; }
    public DateTime?          PrevisaoFechamento { get; private set; }
    public DateTime           CriadoEm          { get; private set; }
    public DateTime           AlteradoEm        { get; private set; }

    public Contato? Contato { get; private set; }
    public Lead?    Lead    { get; private set; }

    private Oportunidade() { }

    public static Oportunidade Criar(string titulo, decimal valor, int probabilidade = 50,
        Guid? contatoId = null, Guid? leadId = null,
        string? descricao = null, DateTime? previsaoFechamento = null,
        EtapaOportunidade etapa = EtapaOportunidade.Prospeccao)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        if (valor < 0)         throw new ArgumentException("Valor não pode ser negativo.", nameof(valor));
        if (contatoId is null && leadId is null)
            throw new ArgumentException("Oportunidade deve ter Contato ou Lead.");
        if (probabilidade is < 0 or > 100)
            throw new ArgumentException("Probabilidade deve estar entre 0 e 100.", nameof(probabilidade));

        return new Oportunidade
        {
            Id                 = Guid.NewGuid(),
            ContatoId          = contatoId,
            LeadId             = leadId,
            Titulo             = titulo.Trim(),
            Valor              = valor,
            Probabilidade      = probabilidade,
            Etapa              = etapa,
            Status             = StatusOportunidade.Aberta,
            Descricao          = descricao?.Trim(),
            PrevisaoFechamento = previsaoFechamento,
            CriadoEm          = DateTime.UtcNow,
            AlteradoEm        = DateTime.UtcNow
        };
    }

    public void Atualizar(string titulo, decimal valor, int probabilidade,
        EtapaOportunidade etapa,
        string? descricao = null, DateTime? previsaoFechamento = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        if (valor < 0) throw new ArgumentException("Valor não pode ser negativo.", nameof(valor));
        if (probabilidade is < 0 or > 100)
            throw new ArgumentException("Probabilidade deve estar entre 0 e 100.", nameof(probabilidade));

        Titulo             = titulo.Trim();
        Valor              = valor;
        Probabilidade      = probabilidade;
        Etapa              = etapa;
        Descricao          = descricao?.Trim();
        PrevisaoFechamento = previsaoFechamento;
        AlteradoEm        = DateTime.UtcNow;
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
