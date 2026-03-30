using events.backend.CrossCutting;

namespace events.backend.Domain.Events;

public sealed record EventCreatedDomainEvent(Guid EventId, string Name) : IDomainEvent;
