using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record CriarOportunidadeCommand(
    Guid?     ContatoId,
    string    Titulo,
    decimal   Valor,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null,
    Guid?     LeadId             = null,
    decimal   Probabilidade      = 0
) : IRequest<Result<OportunidadeDto>>;

public class CriarOportunidadeCommandHandler(
    IOportunidadeRepository repo,
    IContatoRepository contatoRepo,
    ILeadRepository leadRepo,
    IUnitOfWork uow)
    : IRequestHandler<CriarOportunidadeCommand, Result<OportunidadeDto>>
{
    public async Task<Result<OportunidadeDto>> Handle(CriarOportunidadeCommand req, CancellationToken ct)
    {
        string? contatoNome = null;
        if (req.ContatoId.HasValue)
        {
            var contato = await contatoRepo.ObterPorIdAsync(req.ContatoId.Value, ct);
            if (contato is null) return Result.Falha<OportunidadeDto>("Contato não encontrado.");
            contatoNome = contato.Nome;
        }

        string? leadNome = null;
        if (req.LeadId.HasValue)
        {
            var lead = await leadRepo.ObterPorIdAsync(req.LeadId.Value, ct);
            if (lead is null) return Result.Falha<OportunidadeDto>("Lead não encontrado.");
            leadNome = lead.Nome;
        }

        var oportunidade = Oportunidade.Criar(req.ContatoId, req.Titulo, req.Valor,
            req.Descricao, req.PrevisaoFechamento, req.LeadId, req.Probabilidade);

        await repo.AdicionarAsync(oportunidade, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(new OportunidadeDto(
            oportunidade.Id, oportunidade.ContatoId, contatoNome,
            oportunidade.Titulo, oportunidade.Valor, oportunidade.Probabilidade,
            oportunidade.Status, oportunidade.Status.ToString(),
            oportunidade.Descricao, oportunidade.PrevisaoFechamento, oportunidade.CriadoEm,
            oportunidade.LeadId, leadNome
        ));
    }
}
