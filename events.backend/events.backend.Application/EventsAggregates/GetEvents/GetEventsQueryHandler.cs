using Microsoft.EntityFrameworkCore;
using events.backend.Application.Abstractions.Caching;
using events.backend.Application.Abstractions.Messaging;
using events.backend.CrossCutting;
using events.backend.Domain.Database;

namespace events.backend.Application.EventsAggregates.GetEvents;

internal sealed class GetEventsQueryHandler(IUnitOfWork unitOfWork, ICacheService cache)
    : IQueryHandler<GetEventsQuery, GetEventsItem[]>
{
    private const string CacheKey = "events:list:v1";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public async Task<Result<GetEventsItem[]>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync<GetEventsItem[]>(CacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var query = unitOfWork.EventRepository.Queryable();

        var items = await query
            .OrderByDescending(x => x.EventDate)
            .Select(x => new GetEventsItem
            {
                Id = x.Id,
                Name = x.Name,
                EventDate = x.EventDate,
                Place = x.Place,
                Status = x.Status
            })
            .ToArrayAsync(cancellationToken);

        await cache.SetAsync(CacheKey, items, CacheTtl, cancellationToken);
        //return items;
        return Result.Success(items);
    }
}
