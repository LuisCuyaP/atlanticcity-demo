namespace notification.backend.Application.Abstractions.Services;

using notification.backend.Domain.NotificationsAggregates;

public interface IEmailSender
{
    Task SendAsync(NotificationEmail email, CancellationToken cancellationToken = default);
}
