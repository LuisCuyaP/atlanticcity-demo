using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using notification.backend.Application.Abstractions.Repositories;
using notification.backend.Application.Abstractions.Services;
using MassTransit;
using System.Reflection;
using notification.backend.Infrastructure.Database;
using notification.backend.Infrastructure.Services.Email;
using notification.backend.Infrastructure.Persistence.Repositories;

namespace notification.backend.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services
            .AddDatabase(configuration)
            .AddServices()
            .AddMessageBroker(configuration, typeof(DependencyInjection).Assembly)
            ;
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EventsConnectionString") ?? 
            throw new InvalidOperationException("EventsConnectionString not configured");

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailNotificationSettings, ConfigurationEmailNotificationSettings>();
        services.AddTransient<IEmailSender, MailKitEmailSender>();
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
}
