namespace notification.backend.Application.Abstractions.Services;

public interface IEmailNotificationSettings
{
    string DefaultRecipientEmail { get; }
}
