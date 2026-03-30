using events.backend.Domain.Abstractions.Persistence;

namespace events.backend.Domain.EventsAggregates;

public interface IEventRepository : IBaseRepository<Event>
{
}
