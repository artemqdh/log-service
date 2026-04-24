using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Metrics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging;

public class JobRequestSubmitted // тест дата, которую ожидаем
{
    public Guid RequestId { get; set; }
    public string HhUrl { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
}

public class LogConsumer : IConsumer<JobRequestSubmitted>
{
    private readonly ILogProcessor _processor;
    private readonly ILogger<LogConsumer> _logger;

    public LogConsumer(ILogProcessor processor, ILogger<LogConsumer> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JobRequestSubmitted> context)
    {
        var entry = new LogEntry
        {
            Service = "resume-service",
            Level = "Information",
            Message = $"Job requested: {context.Message.HhUrl}",
            CorrelationId = context.Message.CorrelationId.ToString()
        };

        await _processor.ProcessAsync(entry, context.CancellationToken);

        LogMetrics.LogsProcessedTotal.WithLabels("resume-service", "Information").Inc(); // инкремент
        _logger.LogInformation("Log saved for RequestId: {RequestId}", context.Message.RequestId);
    }
}