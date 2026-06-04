using AMR.CRM.Application.DTOs;

namespace AMR.CRM.Application.Oportunidades.Queries;

public record ObterOportunidadeQuery(Guid Id) : IRequest<Result<OportunidadeDto>>;

public class ObterOportunidadeQueryHandler(IOportunidadeRepository repo)
    : IRequestHandler<ObterOportunidadeQuery, Result<OportunidadeDto>>
{
    public async Task<Result<OportunidadeDto>> Handle(ObterOportunidadeQuery req, CancellationToken ct)
    {
        var op = await repo.ObterPorIdAsync(req.Id, ct);
        if (op is null) return Result.Falha<OportunidadeDto>("Oportunidade não encontrada.");
        return Result.Ok(ListarOportunidadesQueryHandler.ToDto(op));
    }
}
