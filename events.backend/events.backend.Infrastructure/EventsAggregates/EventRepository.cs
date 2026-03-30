using events.backend.Domain.EventsAggregates;
using events.backend.Infrastructure.Abstractions.Persistence;
using events.backend.Infrastructure.Database;

namespace events.backend.Infrastructure.EventsAggregates;

internal sealed class EventRepository(ApplicationDbContext context) : BaseRepository<Event>(context), IEventRepository
{
}
