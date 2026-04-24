using Application.Interfaces;
using Application.UseCases;
using Domain.Interfaces;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Prometheus;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ILogRepository, LokiLogRepository>();
        services.AddScoped<ILogProcessor, ProcessLogEntry>();
        services.AddLogging();

        // MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<LogConsumer>();

            x.UsingRabbitMq((context, configurator) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var username = configuration["RabbitMQ:Username"] ?? "admin";
                var password = configuration["RabbitMQ:Password"] ?? "admin123";

                configurator.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                configurator.UsePrometheusMetrics(serviceName: "log-service");

                configurator.ReceiveEndpoint("log-queue", e =>
                {
                    e.ConfigureConsumer<LogConsumer>(context);
                });
            });
        });

        return services;
    }
}