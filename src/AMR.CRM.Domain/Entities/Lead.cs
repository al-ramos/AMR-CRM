using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Lead
{
    public Guid        Id            { get; private set; }
    public string      Nome          { get; private set; } = default!;
    public string      Email         { get; private set; } = default!;
    public string?     Telefone      { get; private set; }
    public string?     Empresa       { get; private set; }
    public StatusLead  Status        { get; private set; }
    public OrigemLead  Origem        { get; private set; }
    public decimal     ValorEstimado { get; private set; }
    public string?     Notas         { get; private set; }
    public DateTime    CriadoEm      { get; private set; }
    public DateTime    AlteradoEm    { get; private set; }

    public ICollection<Oportunidade> Oportunidades { get; private set; } = [];

    private Lead() { }

    public static Lead Criar(string nome, string email, OrigemLead origem,
        decimal valorEstimado = 0, string? telefone = null, string? empresa = null, string? notas = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (valorEstimado < 0) throw new ArgumentException("ValorEstimado não pode ser negativo.", nameof(valorEstimado));

        return new Lead
        {
            Id            = Guid.NewGuid(),
            Nome          = nome.Trim(),
            Email         = email.Trim().ToLowerInvariant(),
            Telefone      = telefone?.Trim(),
            Empresa       = empresa?.Trim(),
            Status        = StatusLead.Novo,
            Origem        = origem,
            ValorEstimado = valorEstimado,
            Notas         = notas?.Trim(),
            CriadoEm      = DateTime.UtcNow,
            AlteradoEm    = DateTime.UtcNow
        };
    }

    public void Atualizar(string nome, string email, OrigemLead origem, decimal valorEstimado,
        string? telefone = null, string? empresa = null, string? notas = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (valorEstimado < 0) throw new ArgumentException("ValorEstimado não pode ser negativo.", nameof(valorEstimado));

        Nome          = nome.Trim();
        Email         = email.Trim().ToLowerInvariant();
        Telefone      = telefone?.Trim();
        Empresa       = empresa?.Trim();
        Origem        = origem;
        ValorEstimado = valorEstimado;
        Notas         = notas?.Trim();
        AlteradoEm    = DateTime.UtcNow;
    }

    public void Qualificar()
    {
        if (Status != StatusLead.Novo)
            throw new InvalidOperationException("Apenas leads Novos podem ser qualificados.");
        Status     = StatusLead.Qualificado;
        AlteradoEm = DateTime.UtcNow;
    }

    public void EnviarProposta()
    {
        if (Status != StatusLead.Qualificado)
            throw new InvalidOperationException("Apenas leads Qualificados podem receber proposta.");
        Status     = StatusLead.Proposta;
        AlteradoEm = DateTime.UtcNow;
    }

    public void Ganhar()
    {
        if (Status is StatusLead.Ganho or StatusLead.Perdido)
            throw new InvalidOperationException("Lead já encerrado.");
        Status     = StatusLead.Ganho;
        AlteradoEm = DateTime.UtcNow;
    }

    public void Perder()
    {
        if (Status is StatusLead.Ganho or StatusLead.Perdido)
            throw new InvalidOperationException("Lead já encerrado.");
        Status     = StatusLead.Perdido;
        AlteradoEm = DateTime.UtcNow;
    }
}
