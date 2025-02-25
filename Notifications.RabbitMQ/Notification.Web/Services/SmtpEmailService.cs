using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Web.Interfaces;
using Notification.Web.Models;

namespace Notification.Web.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpConfig _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpConfig> config, ILogger<SmtpEmailService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }
    public async Task SendEmailAsync(string email, string code)
    {
        var subject = "Код подтверждения";
        var message = $"Ваш код подтверждения: {code}";
        Serilog.Log.Logger.Information($"Processing {message} to {email}");

        await SendEmailAsync(email, subject, message);
    }
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_config.Host, _config.Port, _config.UseSsl);
            await client.AuthenticateAsync(_config.UserName, _config.Password);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(_config.DisplayName, _config.FromAddress));
            mailMessage.To.Add(new MailboxAddress("", email));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain") { Text = message };

            await client.SendAsync(mailMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            throw;
        }
    }
}
