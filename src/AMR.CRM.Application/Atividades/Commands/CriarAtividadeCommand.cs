using AMR.CRM.Application.Atividades.Queries;
using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Atividades.Commands;

public record CriarAtividadeCommand(
    Guid          OportunidadeId,
    TipoAtividade Tipo,
    string        Descricao,
    DateTime      DataHora
) : IRequest<Result<AtividadeDto>>;

public class CriarAtividadeCommandHandler(
    IAtividadeRepository   repo,
    IOportunidadeRepository opRepo,
    IUnitOfWork            uow)
    : IRequestHandler<CriarAtividadeCommand, Result<AtividadeDto>>
{
    public async Task<Result<AtividadeDto>> Handle(CriarAtividadeCommand req, CancellationToken ct)
    {
        var op = await opRepo.ObterPorIdAsync(req.OportunidadeId, ct);
        if (op is null) return Result.Falha<AtividadeDto>("Oportunidade não encontrada.");

        var atividade = Atividade.Criar(req.OportunidadeId, req.Tipo, req.Descricao, req.DataHora);
        await repo.AdicionarAsync(atividade, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(ListarAtividadesQueryHandler.ToDto(atividade));
    }
}
