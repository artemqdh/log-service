using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases;

public class ProcessLogEntry : ILogProcessor
{
    private readonly ILogRepository _repository;

    public ProcessLogEntry(ILogRepository repository)
    {
        _repository = repository;
    }

    public async Task ProcessAsync(LogEntry entry, CancellationToken cancellationToken = default)
    {
        await _repository.SaveAsync(entry, cancellationToken);
    }
}