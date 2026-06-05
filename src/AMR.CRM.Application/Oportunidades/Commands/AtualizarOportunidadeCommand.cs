using AMR.CRM.Application.Interfaces;
using AMR.CRM.Domain.Enums;
using FluentValidation;

namespace AMR.CRM.Application.Oportunidades.Commands;

public record AtualizarOportunidadeCommand(
    Guid              Id,
    string            Titulo,
    decimal           Valor,
    int               Probabilidade,
    EtapaOportunidade Etapa,
    string?           Descricao          = null,
    DateTime?         PrevisaoFechamento = null
) : IRequest<Result>;

public class AtualizarOportunidadeCommandHandler(IOportunidadeRepository repo, IUnitOfWork uow)
    : IRequestHandler<AtualizarOportunidadeCommand, Result>
{
    public async Task<Result> Handle(AtualizarOportunidadeCommand req, CancellationToken ct)
    {
        var op = await repo.ObterPorIdAsync(req.Id, ct);
        if (op is null) return Result.Falha("Oportunidade não encontrada.");

        op.Atualizar(req.Titulo, req.Valor, req.Probabilidade, req.Etapa,
            req.Descricao, req.PrevisaoFechamento);

        repo.Atualizar(op);
        await uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}

public class AtualizarOportunidadeCommandValidator : AbstractValidator<AtualizarOportunidadeCommand>
{
    public AtualizarOportunidadeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(300).WithMessage("Título deve ter no máximo 300 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0).WithMessage("Valor não pode ser negativo.");

        RuleFor(x => x.Probabilidade)
            .InclusiveBetween(0, 100).WithMessage("Probabilidade deve estar entre 0 e 100.");

        RuleFor(x => x.Etapa)
            .IsInEnum().WithMessage("Etapa inválida.");
    }
}
