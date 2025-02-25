using Users.Domain.QueueEntities;

namespace Users.Application.Interfaces;

public interface INotificationQueueService: IAsyncDisposable
{
    Task PublishNotification(NotificationMessage message);
}
