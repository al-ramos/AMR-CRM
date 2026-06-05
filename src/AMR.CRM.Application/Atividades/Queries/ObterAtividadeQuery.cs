using AMR.CRM.Application.Atividades.Commands;
using AMR.CRM.Application.DTOs;

namespace AMR.CRM.Application.Atividades.Queries;

public record ObterAtividadeQuery(Guid Id) : IRequest<AtividadeDto?>;

public class ObterAtividadeQueryHandler(IAtividadeRepository repo)
    : IRequestHandler<ObterAtividadeQuery, AtividadeDto?>
{
    public async Task<AtividadeDto?> Handle(ObterAtividadeQuery req, CancellationToken ct)
    {
        var atividade = await repo.ObterPorIdAsync(req.Id, ct);
        return atividade is null ? null : CriarAtividadeCommandHandler.ToDto(atividade);
    }
}
