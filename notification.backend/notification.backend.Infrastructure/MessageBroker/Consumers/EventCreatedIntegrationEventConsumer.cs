using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using events.backend.Application.EventsAggregates.Events;

namespace notification.backend.Infrastructure.MessageBroker.Consumers;

public sealed class EventCreatedIntegrationEventConsumer(
    IPublisher publisher,
    ILogger<EventCreatedIntegrationEventConsumer> logger)
    : IConsumer<EventCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<EventCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        await publisher.Publish(message, context.CancellationToken);

        logger.LogInformation(
            "EventCreatedIntegrationEvent consumed (MessageId: {MessageId}, EventId: {EventId})",
            message.MessageId,
            message.EventId);
    }
}
