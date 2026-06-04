using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Contato
{
    public Guid              Id        { get; private set; }
    public string            Nome      { get; private set; } = default!;
    public string            Email     { get; private set; } = default!;
    public string?           Telefone  { get; private set; }
    public string?           Empresa   { get; private set; }
    public TipoContato       Tipo      { get; private set; }
    public StatusContato     Status    { get; private set; }
    public string?           Notas     { get; private set; }
    public DateTime          CriadoEm  { get; private set; }
    public DateTime          AlteradoEm { get; private set; }

    public IReadOnlyCollection<Oportunidade> Oportunidades => _oportunidades.AsReadOnly();
    private readonly List<Oportunidade> _oportunidades = [];

    private Contato() { }

    public static Contato Criar(string nome, string email, TipoContato tipo,
        string? telefone = null, string? empresa = null, string? notas = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new Contato
        {
            Id         = Guid.NewGuid(),
            Nome       = nome.Trim(),
            Email      = email.Trim().ToLowerInvariant(),
            Telefone   = telefone?.Trim(),
            Empresa    = empresa?.Trim(),
            Tipo       = tipo,
            Status     = StatusContato.Ativo,
            Notas      = notas?.Trim(),
            CriadoEm   = DateTime.UtcNow,
            AlteradoEm = DateTime.UtcNow
        };
    }

    public void Atualizar(string nome, string email, TipoContato tipo,
        string? telefone = null, string? empresa = null, string? notas = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Nome       = nome.Trim();
        Email      = email.Trim().ToLowerInvariant();
        Telefone   = telefone?.Trim();
        Empresa    = empresa?.Trim();
        Tipo       = tipo;
        Notas      = notas?.Trim();
        AlteradoEm = DateTime.UtcNow;
    }

    public void Inativar()
    {
        Status     = StatusContato.Inativo;
        AlteradoEm = DateTime.UtcNow;
    }

    public void Reativar()
    {
        Status     = StatusContato.Ativo;
        AlteradoEm = DateTime.UtcNow;
    }
}
