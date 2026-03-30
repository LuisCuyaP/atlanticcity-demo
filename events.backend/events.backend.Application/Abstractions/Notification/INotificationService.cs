namespace events.backend.Application.Abstractions.Notification;

public interface INotificationService
{
    Task SendNotificationAsync(string message);
}