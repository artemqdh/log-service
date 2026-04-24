using Application;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Loki;
using Prometheus;
using Microsoft.Extensions.Logging.Abstractions;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "log-service")
    .WriteTo.Console()
    .WriteTo.LokiHttp(new NoAuthCredentials("http://loki:3100"))
    .CreateLogger();

builder.Services.AddSingleton(Log.Logger);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddMetricServer(options =>
{
    options.Port = 1234;
    options.Hostname = "0.0.0.0";
});

var host = builder.Build();

try
{
    Log.Information("Starting LogService...");
    Log.Information("RabbitMQ Host: {Host}", builder.Configuration["RabbitMQ:Host"]);
    Log.Information("Loki URL: {Url}", builder.Configuration["Loki:Url"]);

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "LogService terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}