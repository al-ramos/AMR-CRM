using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Entities;
using AMR.CRM.Domain.Enums;

namespace AMR.CRM.Application.Atividades.Commands;

public record CriarAtividadeCommand(
    string        Titulo,
    TipoAtividade Tipo,
    DateTime      DataPrevista,
    Guid?         LeadId         = null,
    Guid?         OportunidadeId = null,
    string?       Observacao     = null
) : IRequest<Result<AtividadeDto>>;

public class CriarAtividadeCommandHandler(IAtividadeRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarAtividadeCommand, Result<AtividadeDto>>
{
    public async Task<Result<AtividadeDto>> Handle(CriarAtividadeCommand req, CancellationToken ct)
    {
        var atividade = Atividade.Criar(req.Titulo, req.Tipo, req.DataPrevista,
            req.LeadId, req.OportunidadeId, req.Observacao);

        await repo.AdicionarAsync(atividade, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(ToDto(atividade));
    }

    internal static AtividadeDto ToDto(Atividade a) => new(
        a.Id,
        a.Titulo,
        a.Tipo,   a.Tipo.ToString(),
        a.DataPrevista,
        a.Status, a.Status.ToString(),
        a.Observacao,
        a.LeadId,         a.Lead?.Nome,
        a.OportunidadeId, a.Oportunidade?.Titulo,
        a.Status == StatusAtividade.Pendente && a.DataPrevista < DateTime.UtcNow,
        a.CriadoEm,
        a.AlteradoEm
    );
}
