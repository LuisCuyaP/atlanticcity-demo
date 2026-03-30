using notification.backend.Domain.Abstractions;

namespace notification.backend.Domain.Models;

public class ProcessedMessage : Entity
{
    public Guid MessageId { get; set; }
    public string EventType { get; set; } = null!;
    public DateTime ProcessedAt { get; set; }

    public static ProcessedMessage Create(Guid messageId, string eventType)
    {
        return new ProcessedMessage
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            EventType = eventType,
            ProcessedAt = DateTime.UtcNow
        };
    }
}
