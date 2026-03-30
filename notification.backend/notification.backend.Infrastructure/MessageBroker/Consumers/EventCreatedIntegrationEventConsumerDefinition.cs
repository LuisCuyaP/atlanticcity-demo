using MassTransit;

namespace notification.backend.Infrastructure.MessageBroker.Consumers;

public sealed class EventCreatedIntegrationEventConsumerDefinition
    : ConsumerDefinition<EventCreatedIntegrationEventConsumer>
{
    public EventCreatedIntegrationEventConsumerDefinition()
    {
        EndpointName = "notification-event-created";
    }
}
