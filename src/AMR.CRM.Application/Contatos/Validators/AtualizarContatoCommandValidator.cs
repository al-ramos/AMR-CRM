using AMR.CRM.Application.Contatos.Commands;
using FluentValidation;

namespace AMR.CRM.Application.Contatos.Validators;

public class AtualizarContatoCommandValidator : AbstractValidator<AtualizarContatoCommand>
{
    public AtualizarContatoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id do contato é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("E-mail deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de contato inválido.");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres.")
            .When(x => x.Telefone is not null);

        RuleFor(x => x.Empresa)
            .MaximumLength(150).WithMessage("Empresa deve ter no máximo 150 caracteres.")
            .When(x => x.Empresa is not null);

        RuleFor(x => x.Notas)
            .MaximumLength(2000).WithMessage("Notas devem ter no máximo 2000 caracteres.")
            .When(x => x.Notas is not null);
    }
}
