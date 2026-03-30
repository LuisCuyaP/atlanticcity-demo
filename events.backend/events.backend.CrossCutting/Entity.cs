namespace events.backend.CrossCutting;

public abstract class Entity
{
    private readonly List<IDomainEvent> events = [];
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        Id = id;
    }

    protected Entity() { }
    
    public Guid Id { get; set; }

    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null)
        {
            throw new ArgumentNullException(nameof(domainEvent));
        }
        events.Add(domainEvent);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => [.. events];

    public void ClearDomainEvents() => events.Clear();
}
