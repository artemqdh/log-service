using Domain.Entities;

namespace Application.Interfaces;

public interface ILogProcessor
{
    Task ProcessAsync(LogEntry entry, CancellationToken cancellationToken = default);
}