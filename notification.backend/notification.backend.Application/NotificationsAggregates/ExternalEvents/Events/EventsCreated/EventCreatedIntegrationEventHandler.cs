using MediatR;
using Microsoft.Extensions.Logging;
using notification.backend.Application.Abstractions.Repositories;
using notification.backend.Application.Abstractions.Services;
using notification.backend.Domain.Models;
using notification.backend.Domain.NotificationsAggregates;
using System.Text.Json;
using events.backend.Application.EventsAggregates.Events;

namespace events.backend.Application.EventsAggregates.Events;

public sealed class EventCreatedIntegrationEventHandler(
    IEmailSender emailSender,
    IEmailNotificationSettings emailSettings,
    IProcessedMessageRepository processedMessageRepository,
    ILogger<EventCreatedIntegrationEventHandler> logger)
    : INotificationHandler<EventCreatedIntegrationEvent>
{
    public async Task Handle(EventCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        const string eventType = "EventCreated";

        try
        {
            // PASO 1: Chequea idempotencia (¿ya procesé este mensaje?)
            var existing = await processedMessageRepository.GetByMessageIdAndEventTypeAsync(
                notification.MessageId, 
                eventType, 
                cancellationToken);

            if (existing is not null)
            {
                logger.LogInformation(
                    "Message {MessageId} of type {EventType} already processed at {ProcessedAt}. Skipping.",
                    notification.MessageId, eventType, existing.ProcessedAt);
                return;
            }

            // PASO 2: Procesa el evento (envía notificación)
            var to = EmailAddress.Create(emailSettings.DefaultRecipientEmail);
            var payload = JsonSerializer.Serialize(notification, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var body =
                "Hola, este es el cuerpo que llega del otro proyecto:\n\n" +
                payload;

            var email = new NotificationEmail(
                to,
                Subject: "EventCreatedIntegrationEvent",
                BodyText: body);

            //await emailSender.SendAsync(email, cancellationToken);

            logger.LogInformation(
                "EventCreatedIntegrationEvent handled (MessageId: {MessageId}, EventId: {EventId})",
                notification.MessageId,
                notification.EventId);

            // PASO 3: Marca como procesado
            var processedMessage = ProcessedMessage.Create(notification.MessageId, eventType);
            await processedMessageRepository.AddAsync(processedMessage, cancellationToken);
            await processedMessageRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Message {MessageId} marked as processed",
                notification.MessageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error processing EventCreatedIntegrationEvent. MessageId={MessageId}, EventId={EventId}",
                notification.MessageId, notification.EventId);
            throw;
        }
    }
}
