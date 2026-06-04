using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record CriarOportunidadeCommand(
    Guid      ContatoId,
    string    Titulo,
    decimal   Valor,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null
) : IRequest<Result<OportunidadeDto>>;

public class CriarOportunidadeCommandHandler(
    IOportunidadeRepository repo,
    IContatoRepository contatoRepo,
    IUnitOfWork uow)
    : IRequestHandler<CriarOportunidadeCommand, Result<OportunidadeDto>>
{
    public async Task<Result<OportunidadeDto>> Handle(CriarOportunidadeCommand req, CancellationToken ct)
    {
        var contato = await contatoRepo.ObterPorIdAsync(req.ContatoId, ct);
        if (contato is null) return Result.Falha<OportunidadeDto>("Contato não encontrado.");

        var oportunidade = Oportunidade.Criar(req.ContatoId, req.Titulo, req.Valor,
            req.Descricao, req.PrevisaoFechamento);

        await repo.AdicionarAsync(oportunidade, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(new OportunidadeDto(
            oportunidade.Id, oportunidade.ContatoId, contato.Nome,
            oportunidade.Titulo, oportunidade.Valor,
            oportunidade.Status, oportunidade.Status.ToString(),
            oportunidade.Descricao, oportunidade.PrevisaoFechamento, oportunidade.CriadoEm
        ));
    }
}
