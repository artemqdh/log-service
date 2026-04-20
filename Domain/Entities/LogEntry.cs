namespace Domain.Entities;

public class LogEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Service { get; init; } // resume-service
    public required string Level { get; init; } // error, information, warning
    public required string Message { get; init; }
    public required string CorrelationId { get; init; } // GUID of resume-service log
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}