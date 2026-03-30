using events.backend.CrossCutting;
using events.backend.Domain.Events;

namespace events.backend.Domain.EventsAggregates;

public class Event : AuditableEntity
{
    public string Name { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Place { get; set; } = null!;
    public string Status { get; set; } = null!;

    public List<Zone> Zones { get; set; } = [];

    public static Event Create(string name, DateTime eventDate, string place, IReadOnlyCollection<Zone> zones)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(place))
            throw new ArgumentException("Place is required", nameof(place));

        if (zones is null || zones.Count == 0)
            throw new ArgumentException("At least one zone is required", nameof(zones));

        Guid eventId = Guid.NewGuid();

        var entity = new Event
        {
            Id = eventId,
            Name = name,
            EventDate = eventDate,
            Place = place,
            Status = "DRAFT",
            Zones = zones.Select(z => Zone.Create(eventId, z.Name, z.Price, z.Capacity)).ToList()
        };

        entity.RaiseDomainEvent(new EventCreatedDomainEvent(entity.Id, entity.Name));
        return entity;
    }
}
