using Microsoft.EntityFrameworkCore.Storage;
using events.backend.Domain.EventsAggregates;
using System.Data;

namespace events.backend.Domain.Database;

public interface IUnitOfWork : IDisposable
{
    IEventRepository EventRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
    IDbTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
