using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Domain.Entities;

public class Atividade
{
    public Guid          Id             { get; private set; }
    public Guid          OportunidadeId { get; private set; }
    public TipoAtividade Tipo           { get; private set; }
    public string        Descricao      { get; private set; } = default!;
    public DateTime      DataHora       { get; private set; }
    public bool          Concluida      { get; private set; }
    public DateTime      CriadoEm       { get; private set; }
    public DateTime      AlteradoEm     { get; private set; }

    public Oportunidade? Oportunidade { get; private set; }

    private Atividade() { }

    public static Atividade Criar(Guid oportunidadeId, TipoAtividade tipo,
        string descricao, DateTime dataHora)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descricao);

        return new Atividade
        {
            Id             = Guid.NewGuid(),
            OportunidadeId = oportunidadeId,
            Tipo           = tipo,
            Descricao      = descricao.Trim(),
            DataHora       = dataHora,
            Concluida      = false,
            CriadoEm       = DateTime.UtcNow,
            AlteradoEm     = DateTime.UtcNow
        };
    }

    public void Atualizar(TipoAtividade tipo, string descricao, DateTime dataHora)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descricao);

        Tipo       = tipo;
        Descricao  = descricao.Trim();
        DataHora   = dataHora;
        AlteradoEm = DateTime.UtcNow;
    }

    public void ToggleConcluida()
    {
        Concluida  = !Concluida;
        AlteradoEm = DateTime.UtcNow;
    }
}
