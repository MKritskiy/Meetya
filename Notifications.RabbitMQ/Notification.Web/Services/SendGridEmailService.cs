using Notification.Web.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Notification.Web.Services;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(IConfiguration config, ILogger<SendGridEmailService> logger)
    {
        _client = new SendGridClient(config["SendGrid:ApiKey"]);
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string code)
    {
        try
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress("penkov@matvey-dev.ru", "Notification Service"),
                Subject = "Your Verification Code",
                PlainTextContent = $"Your verification code is: {code}"
            };
            msg.AddTo(email);
            var response = await _client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Email send failed: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            throw;
        }
    }
}
