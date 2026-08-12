using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao comunicar com serviço externo (Ollama)");
            await HandleExceptionAsync(
                context,
                HttpStatusCode.BadGateway,
                "Serviço de IA Indisponível",
                "Não foi possível comunicar com o serviço de IA. Verifique se o Ollama está rodando.");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Operação cancelada pelo cliente");
            await HandleExceptionAsync(
                context,
                HttpStatusCode.RequestTimeout,
                "Operação Cancelada",
                "A operação foi cancelada antes de ser concluída.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado na aplicação");
            await HandleExceptionAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Erro Interno",
                "Ocorreu um erro inesperado. Por favor, tente novamente.");
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
