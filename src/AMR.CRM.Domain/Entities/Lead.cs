using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Lead
{
    public Guid        Id             { get; private set; }
    public string      Nome           { get; private set; } = default!;
    public string      Email          { get; private set; } = default!;
    public string?     Telefone       { get; private set; }
    public string?     Empresa        { get; private set; }
    public StatusLead  Status         { get; private set; }
    public OrigemLead  Origem         { get; private set; }
    public decimal     ValorEstimado  { get; private set; }
    public string?     Notas          { get; private set; }
    public DateTime    CriadoEm       { get; private set; }
    public DateTime    AlteradoEm     { get; private set; }

    public int?  OrigemCoreClienteId { get; private set; }

    public IReadOnlyCollection<Oportunidade> Oportunidades => _oportunidades.AsReadOnly();
    private readonly List<Oportunidade> _oportunidades = [];

    private Lead() { }

    public static Lead Criar(string nome, string email, OrigemLead origem,
        string? telefone = null, string? empresa = null,
        decimal valorEstimado = 0m, string? notas = null,
        int? origemCoreClienteId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (valorEstimado < 0) throw new ArgumentException("ValorEstimado não pode ser negativo.", nameof(valorEstimado));

        return new Lead
        {
            Id                  = Guid.NewGuid(),
            Nome                = nome.Trim(),
            Email               = email.Trim().ToLowerInvariant(),
            Telefone            = telefone?.Trim(),
            Empresa             = empresa?.Trim(),
            Status              = StatusLead.Novo,
            Origem              = origem,
            ValorEstimado       = valorEstimado,
            Notas               = notas?.Trim(),
            CriadoEm            = DateTime.UtcNow,
            AlteradoEm          = DateTime.UtcNow,
            OrigemCoreClienteId = origemCoreClienteId,
        };
    }

    public void Atualizar(string nome, string email, OrigemLead origem,
        string? telefone = null, string? empresa = null,
        decimal valorEstimado = 0m, string? notas = null)
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

    public void AvancarStatus(StatusLead novoStatus)
    {
        if (novoStatus == Status)
            throw new InvalidOperationException($"Lead já está no status {Status}.");

        // regras de transição
        if (Status == StatusLead.Ganho || Status == StatusLead.Perdido)
            throw new InvalidOperationException("Lead já encerrado não pode mudar de status.");

        Status     = novoStatus;
        AlteradoEm = DateTime.UtcNow;
    }
}
