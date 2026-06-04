using System.Net;
using System.Text.Json;
using FluentValidation;

namespace AMR.CRM.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validação falhou: {Errors}", ex.Message);
            var erros = ex.Errors.Select(e => e.ErrorMessage).ToList();
            ctx.Response.StatusCode  = (int)HttpStatusCode.BadRequest;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { erros }));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumento inválido");
            await WriteError(ctx, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Operação inválida");
            await WriteError(ctx, HttpStatusCode.UnprocessableEntity, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado");
            await WriteError(ctx, HttpStatusCode.InternalServerError, "Erro interno do servidor.");
        }
    }

    private static Task WriteError(HttpContext ctx, HttpStatusCode status, string message)
    {
        ctx.Response.StatusCode  = (int)status;
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsync(
            JsonSerializer.Serialize(new { erro = message }));
    }
}
