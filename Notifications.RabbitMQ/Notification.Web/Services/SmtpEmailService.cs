using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Web.Interfaces;
using Notification.Web.Models;
using Polly;
using Polly.Retry;

namespace Notification.Web.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpConfig _config;
    private readonly ILogger<SmtpEmailService> _logger;
    private SmtpClient _smtpClient;
    private readonly AsyncRetryPolicy _retryPolicy;
    private bool _disposed;

    public SmtpEmailService(IOptions<SmtpConfig> config, ILogger<SmtpEmailService> logger)
    {
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Retry policy: 3 retries with exponential backoff (2s, 4s, 8s)
        _retryPolicy = Policy
            .Handle<SmtpProtocolException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, waitTime, retryCount, context) =>
                {
                    _logger.LogWarning($"Retry {retryCount} for SMTP error: {exception.Message}. Waiting {waitTime.TotalSeconds} seconds.");
                });
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_smtpClient == null || !_smtpClient.IsConnected || !_smtpClient.IsAuthenticated)
        {
            _smtpClient?.Dispose();
            _smtpClient = new SmtpClient();

            // Use SslOnConnect for port 465, StartTls for port 587
            var secureSocketOptions = _config.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Connecting to SMTP server {Host}:{Port}...", _config.Host, _config.Port);
                await _smtpClient.ConnectAsync(_config.Host, _config.Port, secureSocketOptions, cancellationToken);
                await _smtpClient.AuthenticateAsync(_config.UserName, _config.Password, cancellationToken);
                _logger.LogInformation("Successfully authenticated with SMTP server.");
            });
        }
    }

    public async Task SendEmailAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        var subject = "Код подтверждения";
        var message = $"Ваш код подтверждения: {code}";
        Serilog.Log.Logger.Information($"Processing {message} to {email}");

        await SendEmailAsync(email, subject, message, cancellationToken);
    }
    public async Task SendEmailAsync(string email, string subject, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureConnectedAsync(cancellationToken);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(_config.DisplayName, _config.FromAddress));
            mailMessage.To.Add(new MailboxAddress("", email));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain") { Text = message };

            await _smtpClient.SendAsync(mailMessage, cancellationToken);
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError(ex, "SMTP protocol error while sending email to {Email}", email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            throw new InvalidOperationException("Failed to send email.", ex);
        }
    }
    public void Dispose()
    {
        if (!_disposed)
        {
            _smtpClient?.Disconnect(true);
            _smtpClient?.Dispose();
            _disposed = true;
        }
    }
}
