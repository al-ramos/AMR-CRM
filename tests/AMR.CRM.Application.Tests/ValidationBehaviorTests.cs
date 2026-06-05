using AMR.CRM.Application.Behaviors;
using AMR.CRM.Application.DTOs;
using AMR.CRM.Application.Leads.Commands;
using AMR.CRM.Application.Leads.Commands.Validators;
using AMR.CRM.Domain.Enums;
using AMR.CRM.Shared.Results;
using FluentValidation;

namespace AMR.CRM.Application.Tests;

public class ValidationBehaviorTests
{
    private static readonly CriarLeadCommandValidator Validator = new();

    // ── DeveLancarValidationException_QuandoInvalido ─────────────────────────
    [Fact]
    public async Task DeveLancarValidationException_QuandoEmailInvalido()
    {
        var behavior = new ValidationBehavior<CriarLeadCommand, Result<LeadDto>>([Validator]);
        var cmd      = new CriarLeadCommand("Empresa Válida", "email-sem-arroba", OrigemLead.Website);

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(cmd, () => Task.FromResult(Result.Falha<LeadDto>("não deve chegar aqui")),
                CancellationToken.None));
    }

    [Fact]
    public async Task DeveLancarValidationException_QuandoNomeVazio()
    {
        var behavior = new ValidationBehavior<CriarLeadCommand, Result<LeadDto>>([Validator]);
        var cmd      = new CriarLeadCommand("", "valido@empresa.com", OrigemLead.LinkedIn);

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(cmd, () => Task.FromResult(Result.Falha<LeadDto>("não deve chegar aqui")),
                CancellationToken.None));
    }

    // ── DevePassar_QuandoValido ──────────────────────────────────────────────
    [Fact]
    public async Task DevePassar_QuandoValido()
    {
        var behavior   = new ValidationBehavior<CriarLeadCommand, Result<LeadDto>>([Validator]);
        var cmd        = new CriarLeadCommand("Empresa Válida", "empresa@valido.com", OrigemLead.Website,
            ValorEstimado: 5_000m);
        var nextCalled = false;

        await behavior.Handle(cmd, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Falha<LeadDto>("stub"));
        }, CancellationToken.None);

        Assert.True(nextCalled);
    }
}
