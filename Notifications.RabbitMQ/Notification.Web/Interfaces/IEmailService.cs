namespace Notification.Web.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string email, string code, CancellationToken cancellationToken = default);
}
