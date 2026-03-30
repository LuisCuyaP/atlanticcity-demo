namespace notification.backend.Domain.NotificationsAggregates;

public sealed record NotificationEmail(
    EmailAddress To,
    string Subject,
    string BodyText)
{
    public static NotificationEmail FromEventCreated(
        EmailAddress to,
        Guid eventId,
        string name,
        DateTimeOffset occurredAt,
        Guid correlationId,
        int version)
    {
        var subject = $"Event created: {name}";
        var body =
            $"Event Created\n" +
            $"- EventId: {eventId}\n" +
            $"- Name: {name}\n" +
            $"- OccurredAt: {occurredAt:O}\n" +
            $"- CorrelationId: {correlationId}\n" +
            $"- Version: {version}\n";

        return new NotificationEmail(to, subject, body);
    }
}
