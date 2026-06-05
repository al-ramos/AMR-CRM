using AMR.CRM.Application.Atividades.Commands;
using FluentValidation;

namespace AMR.CRM.Application.Atividades.Commands.Validators;

public class CriarAtividadeCommandValidator : AbstractValidator<CriarAtividadeCommand>
{
    public CriarAtividadeCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de atividade inválido.");

        RuleFor(x => x.DataPrevista)
            .NotEmpty().WithMessage("Data prevista é obrigatória.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000).When(x => x.Observacao is not null);
    }
}
