using System.Text;
using events.backend.Application.Abstractions.Authentication;
using events.backend.Domain.Database;
using events.backend.Infrastructure.Abstractions.Authentication;
using events.backend.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using events.backend.Application.Abstractions.Notification;
using events.backend.Infrastructure.Abstractions.Notification;
using events.backend.Domain.EventsAggregates;
using events.backend.Infrastructure.EventsAggregates;
using MassTransit;
using System.Reflection;
using events.backend.Application.Abstractions.Messaging;
using events.backend.Infrastructure.Abstractions.Messaging;
using events.backend.Application.Abstractions.Caching;
using events.backend.Infrastructure.Abstractions.Caching;

namespace events.backend.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services
            .AddDatabase(configuration, environment)
            .AddRepositories()
            .AddServices()
            .AddCache(configuration)
            .AddMessageBroker(configuration, Assembly.GetExecutingAssembly())
            .AddAuthenticationInternal(configuration);
        return services;
    }


    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        string sqlConnectionString = configuration["EventsConnectionString"] ?? throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {            
            options.UseSqlServer(sqlConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            var interceptor = sp.GetRequiredService<AuditoriaInterceptor>();
            options.AddInterceptors(interceptor);
        });
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<AuditoriaInterceptor>();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {        
        services.AddSingleton<INotificationService, NotificationService>();        
        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();
        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis"];
        });

        services.AddSingleton<ICacheService, DistributedCacheService>();
        return services;
    }

    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            if (assembly != null)
                config.AddConsumers(assembly);

            config.UsingRabbitMq((context, configurator) =>
            {
                // Exponential retry policy: 3 retries with backoff (200ms-5s)
                configurator.UseMessageRetry(r => r.Exponential(
                    retryLimit: 3,
                    minInterval: TimeSpan.FromMilliseconds(200),
                    maxInterval: TimeSpan.FromSeconds(5),
                    intervalDelta: TimeSpan.FromMilliseconds(200)));

                configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                {
                    host.Username(configuration["MessageBroker:UserName"]);
                    host.Password(configuration["MessageBroker:Password"]);
                });
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddHttpContextAccessor();
        services.AddAuthorization();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        return services;
    }

}
