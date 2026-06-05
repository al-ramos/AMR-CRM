using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Atividade
{
    public Guid            Id             { get; private set; }
    public string          Titulo         { get; private set; } = default!;
    public TipoAtividade   Tipo           { get; private set; }
    public DateTime        DataPrevista   { get; private set; }
    public StatusAtividade Status         { get; private set; }
    public string?         Observacao     { get; private set; }
    public Guid?           LeadId         { get; private set; }
    public Guid?           OportunidadeId { get; private set; }
    public DateTime        CriadoEm      { get; private set; }
    public DateTime        AlteradoEm    { get; private set; }

    public Lead?         Lead         { get; private set; }
    public Oportunidade? Oportunidade { get; private set; }

    private Atividade() { }

    public static Atividade Criar(string titulo, TipoAtividade tipo, DateTime dataPrevista,
        Guid? leadId = null, Guid? oportunidadeId = null, string? observacao = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);

        return new Atividade
        {
            Id             = Guid.NewGuid(),
            Titulo         = titulo.Trim(),
            Tipo           = tipo,
            DataPrevista   = dataPrevista,
            Status         = StatusAtividade.Pendente,
            Observacao     = observacao?.Trim(),
            LeadId         = leadId,
            OportunidadeId = oportunidadeId,
            CriadoEm      = DateTime.UtcNow,
            AlteradoEm    = DateTime.UtcNow,
        };
    }

    public void Atualizar(string titulo, TipoAtividade tipo, DateTime dataPrevista,
        StatusAtividade status, string? observacao = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);

        Titulo       = titulo.Trim();
        Tipo         = tipo;
        DataPrevista = dataPrevista;
        Status       = status;
        Observacao   = observacao?.Trim();
        AlteradoEm  = DateTime.UtcNow;
    }
}
