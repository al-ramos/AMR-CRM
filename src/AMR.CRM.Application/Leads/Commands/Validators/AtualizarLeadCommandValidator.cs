using FluentValidation;

namespace AMR.CRM.Application.Leads.Commands.Validators;

public class AtualizarLeadCommandValidator : AbstractValidator<AtualizarLeadCommand>
{
    public AtualizarLeadCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id é obrigatório.");

        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("E-mail deve ter no máximo 200 caracteres.");

        RuleFor(x => x.ValorEstimado)
            .GreaterThanOrEqualTo(0).WithMessage("ValorEstimado não pode ser negativo.");
    }
}
