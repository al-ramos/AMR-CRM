using FluentValidation;

namespace AMR.CRM.Application.Oportunidades.Commands.Validators;

public class CriarOportunidadeCommandValidator : AbstractValidator<CriarOportunidadeCommand>
{
    public CriarOportunidadeCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(300).WithMessage("Título deve ter no máximo 300 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0).WithMessage("Valor não pode ser negativo.");

        RuleFor(x => x.Probabilidade)
            .InclusiveBetween(0, 100).WithMessage("Probabilidade deve estar entre 0 e 100.");

        RuleFor(x => x.Etapa)
            .IsInEnum().WithMessage("Etapa inválida.");

        RuleFor(x => x)
            .Must(x => x.ContatoId.HasValue || x.LeadId.HasValue)
            .WithMessage("Informe Contato ou Lead.");
    }
}
