using AMR.CRM.Application.Oportunidades.Commands;
using AMR.CRM.Domain.Enums;
using FluentValidation;

namespace AMR.CRM.Application.Oportunidades.Validators;

public class AvancarOportunidadeCommandValidator : AbstractValidator<AvancarOportunidadeCommand>
{
    public AvancarOportunidadeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id da oportunidade é obrigatório.");

        RuleFor(x => x.NovoStatus)
            .IsInEnum().WithMessage("Status inválido.")
            .NotEqual(StatusOportunidade.Aberta).WithMessage("Não é possível retornar ao status Aberta.");
    }
}
