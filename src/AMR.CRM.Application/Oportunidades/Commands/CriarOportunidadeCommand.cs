using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record CriarOportunidadeCommand(
    string    Titulo,
    decimal   Valor,
    int       Probabilidade      = 50,
    Guid?     ContatoId          = null,
    Guid?     LeadId             = null,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null
) : IRequest<Result<OportunidadeDto>>;

public class CriarOportunidadeCommandHandler(
    IOportunidadeRepository repo,
    IContatoRepository      contatoRepo,
    ILeadRepository         leadRepo,
    IUnitOfWork             uow)
    : IRequestHandler<CriarOportunidadeCommand, Result<OportunidadeDto>>
{
    public async Task<Result<OportunidadeDto>> Handle(CriarOportunidadeCommand req, CancellationToken ct)
    {
        if (req.ContatoId is null && req.LeadId is null)
            return Result.Falha<OportunidadeDto>("Informe Contato ou Lead.");

        string?  contatoNome = null;
        string?  leadNome    = null;

        if (req.ContatoId.HasValue)
        {
            var contato = await contatoRepo.ObterPorIdAsync(req.ContatoId.Value, ct);
            if (contato is null) return Result.Falha<OportunidadeDto>("Contato não encontrado.");
            contatoNome = contato.Nome;
        }

        if (req.LeadId.HasValue)
        {
            var lead = await leadRepo.ObterPorIdAsync(req.LeadId.Value, ct);
            if (lead is null) return Result.Falha<OportunidadeDto>("Lead não encontrado.");
            leadNome = lead.Nome;
        }

        var op = Oportunidade.Criar(req.Titulo, req.Valor, req.Probabilidade,
            req.ContatoId, req.LeadId, req.Descricao, req.PrevisaoFechamento);

        await repo.AdicionarAsync(op, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(new OportunidadeDto(
            op.Id, op.ContatoId, contatoNome, op.LeadId, leadNome,
            op.Titulo, op.Valor, op.Probabilidade,
            op.Status, op.Status.ToString(),
            op.Descricao, op.PrevisaoFechamento, op.CriadoEm
        ));
    }
}
