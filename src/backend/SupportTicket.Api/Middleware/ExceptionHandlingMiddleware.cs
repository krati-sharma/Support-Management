using System.Text.Json;
using SupportTicket.Domain.Exceptions;

namespace SupportTicket.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                validationException.Message,
                validationException.Errors),
            InvalidStatusTransitionException transitionException => (
                StatusCodes.Status400BadRequest,
                "Invalid status transition",
                transitionException.Message,
                (IDictionary<string, string[]>?)null),
            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                notFoundException.Message,
                (IDictionary<string, string[]>?)null),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "An unexpected error occurred. Please try again later.",
                (IDictionary<string, string[]>?)null)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Handled application exception: {Title}", title);
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        object payload = errors is null
            ? new
            {
                type = "about:blank",
                title,
                status = statusCode,
                detail
            }
            : new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title,
                status = statusCode,
                detail,
                errors
            };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
