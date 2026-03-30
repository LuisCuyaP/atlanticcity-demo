using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using notification.backend.Application.Abstractions.Services;
using notification.backend.Domain.NotificationsAggregates;

namespace notification.backend.Infrastructure.Services.Email;

internal sealed class MailKitEmailSender(IConfiguration configuration, ILogger<MailKitEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(NotificationEmail email, CancellationToken cancellationToken = default)
    {
        var fromEmail = configuration["Email:FromEmail"] ?? throw new ArgumentNullException("Email:FromEmail");
        var fromName = configuration["Email:FromName"] ?? string.Empty;

        var host = configuration["Email:Smtp:Host"] ?? throw new ArgumentNullException("Email:Smtp:Host");
        var port = int.TryParse(configuration["Email:Smtp:Port"], out var parsedPort) ? parsedPort : 587;
        var userName = configuration["Email:Smtp:UserName"] ?? throw new ArgumentNullException("Email:Smtp:UserName");
        var password = configuration["Email:Smtp:Password"] ?? throw new ArgumentNullException("Email:Smtp:Password");
        var useStartTls = !bool.TryParse(configuration["Email:Smtp:UseStartTls"], out var parsedTls) || parsedTls;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress("", email.To.Value));
        message.Subject = email.Subject;

        var bodyBuilder = new BodyBuilder { TextBody = email.BodyText };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(host, port, useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);
            await client.AuthenticateAsync(userName, password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        logger.LogInformation("Email sent to {EmailAddress} with subject {Subject}", email.To.Value, email.Subject);
    }
}
