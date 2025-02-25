using Notification.Web.Interfaces;

namespace Notification.Web.Services;

public class TwilioSmsService : ISmsService
{
    public Task SendSmsAsync(string phone, string code)
    {
        throw new NotImplementedException();
    }
}
