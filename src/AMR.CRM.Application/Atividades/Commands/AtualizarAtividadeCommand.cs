using AMR.CRM.Application.Atividades.Queries;
using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Atividades.Commands;

public record AtualizarAtividadeCommand(
    Guid          Id,
    TipoAtividade Tipo,
    string        Descricao,
    DateTime      DataHora
) : IRequest<Result<AtividadeDto>>;

public class AtualizarAtividadeCommandHandler(
    IAtividadeRepository repo,
    IUnitOfWork          uow)
    : IRequestHandler<AtualizarAtividadeCommand, Result<AtividadeDto>>
{
    public async Task<Result<AtividadeDto>> Handle(AtualizarAtividadeCommand req, CancellationToken ct)
    {
        var a = await repo.ObterPorIdAsync(req.Id, ct);
        if (a is null) return Result.Falha<AtividadeDto>("Atividade não encontrada.");

        a.Atualizar(req.Tipo, req.Descricao, req.DataHora);
        repo.Atualizar(a);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(ListarAtividadesQueryHandler.ToDto(a));
    }
}
