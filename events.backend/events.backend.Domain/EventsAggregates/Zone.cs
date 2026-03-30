using events.backend.CrossCutting;

namespace events.backend.Domain.EventsAggregates;

public class Zone : AuditableEntity
{
    public Guid EventId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Capacity { get; set; }

    public static Zone Create(Guid eventId, string name, decimal price, int capacity)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId is required", nameof(eventId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Zone name is required", nameof(name));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be >= 0");

        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be > 0");

        return new Zone
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Name = name,
            Price = price,
            Capacity = capacity
        };
    }
}
