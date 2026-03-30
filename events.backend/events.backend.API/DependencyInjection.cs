using Microsoft.OpenApi.Models;
using Serilog;

namespace events.backend.API;

public static class DependencyInjection
{
    #region Configuration
    public static WebApplicationBuilder ConfigureEvents(this WebApplicationBuilder builder)
    {
        builder
            .AddConfigureCors()
            .AddSerilog();

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

    private static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "EVENTS API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
            { 
                In = ParameterLocation.Header,
                Description = "Paste a JWT access token (without the 'Bearer ' prefix)",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
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

    public static IApplicationBuilder UseSwaggerEVENTS(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "EVENTS API V1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}

