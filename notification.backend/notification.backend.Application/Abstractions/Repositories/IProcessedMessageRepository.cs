using notification.backend.Domain.Models;

namespace notification.backend.Application.Abstractions.Repositories;

public interface IProcessedMessageRepository
{
    Task<ProcessedMessage?> GetByMessageIdAndEventTypeAsync(Guid messageId, string eventType, CancellationToken cancellationToken = default);
    Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
