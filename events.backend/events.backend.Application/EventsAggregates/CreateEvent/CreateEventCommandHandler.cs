using events.backend.Application.Abstractions.Messaging;
using events.backend.Application.Abstractions.Caching;
using events.backend.CrossCutting;
using events.backend.Domain.Database;
using events.backend.Domain.EventsAggregates;
using Event = events.backend.Domain.EventsAggregates.Event;

namespace events.backend.Application.EventsAggregates.CreateEvent;

internal sealed class CreateEventCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : ICommandHandler<CreateEventCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Zone> zoneTemplates = request.Zones
            .Select(z => new Zone
            {
                Name = z.Name,
                Price = z.Price,
                Capacity = z.Capacity
            })
            .ToArray();

        var entity = Event.Create(request.Name, request.EventDate, request.Place, zoneTemplates);
        unitOfWork.EventRepository.Add(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync("events:list:v1", cancellationToken);

        return entity.Id;
    }
}
