using Microsoft.OpenApi.Models;
using Serilog;

namespace notification.backend.API;

public static class DependencyInjection
{
    #region Configuration
    public static WebApplicationBuilder ConfigureBackendNotification(this WebApplicationBuilder builder)
    {
        builder
            .AddConfigureCors()
            .AddSerilog();

        return builder;
    }

    private static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext());

        return builder;
    }

    private static WebApplicationBuilder AddConfigureCors(this WebApplicationBuilder builder)
    {
        var domains = builder.Configuration["DomainsAllowsCors"]?.Split(";") ?? [];
        domains = [.. domains.Where(x => x.Length > 0)];
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", 
                builder =>
                {
                    if (domains.Length != 0)
                        builder.WithOrigins(domains).AllowAnyHeader().AllowAnyMethod();
                });
        });

        return builder;
    }

    #endregion

    #region Add Presentation

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSwagger();
        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BACKEND NOTIFICATION API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
            { 
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            c.CustomSchemaIds(t => t.FullName?.Replace("+", "."));

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
        return services;
    }

    #endregion

    public static IApplicationBuilder UseSwaggerNotification(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BACKEND NOTIFICATION API V1");
            c.RoutePrefix = string.Empty;
        });

        return app;
    }
}

