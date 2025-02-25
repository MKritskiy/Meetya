namespace Notification.Web.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string phone, string code);
}
