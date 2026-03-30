using MediatR;
using events.backend.Application.Abstractions.Messaging;
using events.backend.Application.EventsAggregates.Events;
using events.backend.Domain.Events;

namespace events.backend.Application.EventsAggregates.CreateEvent;

internal sealed class EventCreatedDomainEventHandler(IMessagePublisher messagePublisher)
    : INotificationHandler<EventCreatedDomainEvent>
{
    public Task Handle(EventCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EventCreatedIntegrationEvent(
            MessageId: Guid.NewGuid(),
            EventId: notification.EventId,
            Name: notification.Name,
            OccurredAt: DateTimeOffset.UtcNow,
            CorrelationId: Guid.NewGuid(),
            Version: 1);

        return messagePublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
