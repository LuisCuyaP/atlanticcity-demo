using events.backend.Application.Abstractions.Messaging;

namespace events.backend.Application.EventsAggregates.GetEvents;

public sealed class GetEventsQuery : IQuery<GetEventsItem[]>
{
}
