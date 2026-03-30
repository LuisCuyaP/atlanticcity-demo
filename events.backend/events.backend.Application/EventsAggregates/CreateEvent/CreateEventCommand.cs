using events.backend.Application.Abstractions.Messaging;

namespace events.backend.Application.EventsAggregates.CreateEvent;

public sealed class CreateEventCommand : ICommand<Guid>
{
    public string Name { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Place { get; set; } = null!;

    public List<CreateEventZone> Zones { get; set; } = [];
}

public sealed class CreateEventZone
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Capacity { get; set; }
}
