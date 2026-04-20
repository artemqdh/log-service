using Domain.Entities;
using Domain.Interfaces;
using Serilog;

namespace Infrastructure.Persistence;

public class LokiLogRepository : ILogRepository
{
    private readonly ILogger _logger;

    public LokiLogRepository(ILogger logger)
    {
        _logger = logger;
    }

    public Task SaveAsync(LogEntry entry, CancellationToken cancellationToken = default)
    {
        _logger.Information(
            "[{Service}] [{CorrelationId}] [{Level}] {Message}",
            entry.Service,
            entry.CorrelationId,
            entry.Level,
            entry.Message);

        return Task.CompletedTask;
    }
}