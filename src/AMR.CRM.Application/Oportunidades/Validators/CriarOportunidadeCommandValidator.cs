using AMR.CRM.Application.Oportunidades.Commands;
using FluentValidation;

namespace AMR.CRM.Application.Oportunidades.Validators;

public class CriarOportunidadeCommandValidator : AbstractValidator<CriarOportunidadeCommand>
{
    public CriarOportunidadeCommandValidator()
    {
        RuleFor(x => x.ContatoId)
            .NotEmpty().WithMessage("ContatoId é obrigatório.");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0).WithMessage("Valor não pode ser negativo.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000).WithMessage("Descrição deve ter no máximo 2000 caracteres.")
            .When(x => x.Descricao is not null);

        RuleFor(x => x.PrevisaoFechamento)
            .GreaterThan(DateTime.UtcNow).WithMessage("Previsão de fechamento deve ser uma data futura.")
            .When(x => x.PrevisaoFechamento is not null);
    }
}
