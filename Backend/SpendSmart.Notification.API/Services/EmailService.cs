using MailKit.Net.Smtp;
using MimeKit;

namespace SpendSmart.Notification.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var host = _config["Email:Host"];
            var port = _config.GetValue<int>("Email:Port");
            var user = _config["Email:Username"];
            var pass = _config["Email:Password"];

            if (string.IsNullOrEmpty(host))
            {
                _logger.LogWarning($"Email config missing. Simulated Email to {to}: {subject}");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SpendSmart", "noreply@spendsmart.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, false);
            await client.AuthenticateAsync(user, pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
        }
    }
}
