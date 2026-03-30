namespace events.backend.Application.EventsAggregates.Events;

public sealed record EventCreatedIntegrationEvent(
    Guid MessageId,
    Guid EventId,
    string Name,
    DateTimeOffset OccurredAt,
    Guid CorrelationId,
    int Version);
