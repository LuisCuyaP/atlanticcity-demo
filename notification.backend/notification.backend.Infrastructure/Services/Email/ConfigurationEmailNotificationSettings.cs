using Microsoft.Extensions.Configuration;
using notification.backend.Application.Abstractions.Services;

namespace notification.backend.Infrastructure.Services.Email;

internal sealed class ConfigurationEmailNotificationSettings(IConfiguration configuration) : IEmailNotificationSettings
{
    public string DefaultRecipientEmail =>
        configuration["Email:DefaultRecipientEmail"]
        ?? throw new ArgumentNullException("Email:DefaultRecipientEmail");
}
