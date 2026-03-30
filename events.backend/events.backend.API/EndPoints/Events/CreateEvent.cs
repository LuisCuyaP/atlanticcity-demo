using MediatR;
using events.backend.API.EndPoints;
using events.backend.API.Extensions;
using events.backend.Application.EventsAggregates.CreateEvent;

namespace events.backend.API.EndPoints.Events;

internal sealed class CreateEvent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/events", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateEventCommand
            {
                Name = request.Name,
                EventDate = request.EventDate,
                Place = request.Place,
                Zones = (request.Zones ?? [])
                    .Select(z => new CreateEventZone { Name = z.Name, Price = z.Price, Capacity = z.Capacity })
                    .ToList()
            };

            var result = await sender.Send(command, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Events)
        .RequireRateLimiting("events-write")
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public string Name { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string Place { get; set; } = null!;
        public List<ZoneRequest> Zones { get; set; } = [];
    }

    internal sealed class ZoneRequest
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
