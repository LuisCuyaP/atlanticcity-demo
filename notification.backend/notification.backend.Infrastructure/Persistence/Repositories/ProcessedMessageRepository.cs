using Microsoft.EntityFrameworkCore;
using notification.backend.Application.Abstractions.Repositories;
using notification.backend.Domain.Models;
using notification.backend.Infrastructure.Database;

namespace notification.backend.Infrastructure.Persistence.Repositories;

internal sealed class ProcessedMessageRepository(NotificationDbContext context) : IProcessedMessageRepository
{
    public async Task<ProcessedMessage?> GetByMessageIdAndEventTypeAsync(Guid messageId, string eventType, CancellationToken cancellationToken = default)
    {
        return await context.ProcessedMessages
            .FirstOrDefaultAsync(x => x.MessageId == messageId && x.EventType == eventType, cancellationToken);
    }

    public async Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default)
    {
        await context.ProcessedMessages.AddAsync(processedMessage, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
