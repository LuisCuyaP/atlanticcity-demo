using MediatR;
using events.backend.API.EndPoints;
using events.backend.API.Extensions;
using events.backend.Application.EventsAggregates.GetEvents;

namespace events.backend.API.EndPoints.Events;

internal sealed class GetEvents : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/events", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetEventsQuery(), cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Events)
        .RequireAuthorization();
    }
}
