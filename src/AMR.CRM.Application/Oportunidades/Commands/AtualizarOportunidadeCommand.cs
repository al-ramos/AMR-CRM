using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record AtualizarOportunidadeCommand(
    Guid      Id,
    string    Titulo,
    decimal   Valor,
    decimal   Probabilidade      = 0,
    string?   Descricao          = null,
    DateTime? PrevisaoFechamento = null
) : IRequest<Result<OportunidadeDto>>;

public class AtualizarOportunidadeCommandHandler(
    IOportunidadeRepository repo,
    IUnitOfWork uow)
    : IRequestHandler<AtualizarOportunidadeCommand, Result<OportunidadeDto>>
{
    public async Task<Result<OportunidadeDto>> Handle(AtualizarOportunidadeCommand req, CancellationToken ct)
    {
        var op = await repo.ObterPorIdAsync(req.Id, ct);
        if (op is null) return Result.Falha<OportunidadeDto>("Oportunidade não encontrada.");

        op.Atualizar(req.Titulo, req.Valor, req.Probabilidade, req.Descricao, req.PrevisaoFechamento);
        repo.Atualizar(op);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(new OportunidadeDto(
            op.Id, op.ContatoId, op.Contato?.Nome,
            op.Titulo, op.Valor, op.Probabilidade,
            op.Status, op.Status.ToString(),
            op.Descricao, op.PrevisaoFechamento, op.CriadoEm,
            op.LeadId, op.Lead?.Nome
        ));
    }
}
