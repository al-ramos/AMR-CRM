using AMR.CRM.Application.Contatos.Commands;
using FluentValidation;

namespace AMR.CRM.Application.Contatos.Validators;

public class InativarContatoCommandValidator : AbstractValidator<InativarContatoCommand>
{
    public InativarContatoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id do contato é obrigatório.");
    }
}
