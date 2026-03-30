using events.backend.Application.Abstractions.Notification;

namespace events.backend.Infrastructure.Abstractions.Notification;

internal sealed class NotificationService : INotificationService
{
    public Task SendNotificationAsync(string message)
    {        
        Console.WriteLine($"Notification sent: {message}");
        return Task.CompletedTask;
    }
    
}