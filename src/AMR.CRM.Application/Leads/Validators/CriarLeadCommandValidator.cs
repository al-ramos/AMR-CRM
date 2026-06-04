using AMR.CRM.Application.Leads.Commands;
using FluentValidation;

namespace AMR.CRM.Application.Leads.Validators;

public class CriarLeadCommandValidator : AbstractValidator<CriarLeadCommand>
{
    public CriarLeadCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Origem).IsInEnum();
        RuleFor(x => x.ValorEstimado).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Telefone).MaximumLength(20).When(x => x.Telefone is not null);
        RuleFor(x => x.Empresa).MaximumLength(150).When(x => x.Empresa is not null);
        RuleFor(x => x.Notas).MaximumLength(2000).When(x => x.Notas is not null);
    }
}
