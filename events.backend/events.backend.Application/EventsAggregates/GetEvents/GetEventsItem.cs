namespace events.backend.Application.EventsAggregates.GetEvents;

public sealed class GetEventsItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime EventDate { get; init; }
    public string Place { get; init; } = null!;
    public string Status { get; init; } = null!;
}
