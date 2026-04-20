using Application;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Loki;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "log-service")
    .CreateLogger();

builder.Services.AddSingleton(Log.Logger);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

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