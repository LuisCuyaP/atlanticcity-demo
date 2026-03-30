

using Microsoft.Extensions.Logging;

namespace events.backend.API.Middlewares;

public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionLoggingMiddleware> _logger;

    public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
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
            _logger.LogError(ex,
                "Unhandled exception. Path={Path} Method={Method} User={User}",
                context.Request.Path.Value,
                context.Request.Method,
                context.User.Identity?.Name ?? "Anonymous");
            throw;
        }
    }
}