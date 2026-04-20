using Domain.Entities;

namespace Domain.Interfaces;

public interface ILogRepository
{
    Task SaveAsync(LogEntry entry, CancellationToken cancellationToken = default);
}