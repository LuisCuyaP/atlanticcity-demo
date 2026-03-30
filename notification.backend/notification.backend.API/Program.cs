using notification.backend.API;
using notification.backend.Infrastructure;
using notification.backend.API.Middlewares;
using Serilog;
using notification.backend.Application;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureBackendNotification();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProblemDetails();

builder.Services
    .AddPresentation()
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApplication();
    
var app = builder.Build();

app.UseSerilogRequestLogging();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Referrer-Policy", "same-origin");
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1");

    await next(context);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerNotification();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
//app.UseAuthorization();
app.UseMiddleware<ExceptionLoggingMiddleware>();
app.Run();
