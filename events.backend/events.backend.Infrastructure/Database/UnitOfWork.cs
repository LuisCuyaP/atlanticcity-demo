using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using events.backend.Application.Abstractions.Authentication;
using events.backend.CrossCutting;
using events.backend.Domain.Database;
using events.backend.Domain.EventsAggregates;
using System.Data;

namespace events.backend.Infrastructure.Database;

internal sealed class UnitOfWork(ApplicationDbContext context,
    IUserContext userContext,
    IEventRepository eventRepository,
    IPublisher publisher) : IUnitOfWork
{
    public IEventRepository EventRepository { get; } = eventRepository;

    public int SaveChanges() => context.SaveChanges();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.Now;
        var entries = context.ChangeTracker.Entries().Where(x => x.Entity is AuditableEntity);
        foreach (var entry in entries)
        {
            if (entry.Entity is AuditableEntity auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.AddCreateInfo(userContext.RegistrationId ?? string.Empty, now);
                        break;

                    case EntityState.Modified:
                        auditableEntity.AddModifyInfo(userContext.RegistrationId ?? string.Empty, now);
                        break;
                }
            }
        }

        var result = await context.SaveChangesAsync(cancellationToken);

        // Publish (and clear) domain events only after the transaction succeeds.
        await PublishDomainEventsAsync(cancellationToken);
        return result;
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public IDbTransaction BeginTransaction()
    {
        var dbConnection = context.Database.GetDbConnection();
        if (dbConnection.State == ConnectionState.Closed)
        {
            dbConnection.Open();
        }
        return dbConnection.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = context.ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}