using events.backend.API;
using events.backend.Infrastructure;
using events.backend.API.Middlewares;
using events.backend.API.Extensions;
using events.backend.Application;
using events.backend.Infrastructure.Database;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureEvents();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddCheck("api-health", () => HealthCheckResult.Healthy("API is running"), tags: ["live"]);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("events-write", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});

builder.Services
    .AddPresentation()
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApplication()
    .AddEndpoints(AssemblyReference.Assembly);
    
var app = builder.Build();

app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Referrer-Policy", "same-origin");
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1");

    await next(context);
});

app.UseSwaggerEVENTS();

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionLoggingMiddleware>();

app.MapHealthChecks("/health");
app.MapEndpoints();
app.Run();
