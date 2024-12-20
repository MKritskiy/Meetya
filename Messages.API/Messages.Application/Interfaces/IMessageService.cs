using Messages.Domain.Entities;

namespace Messages.Application.Interfaces;

public interface IMessageService
{
    Task<Message> GetMessage(int id);
    Task DeleteMessage(int id);
    // Возвращает сообщения для ивента с id равным eventId
    // В количестве countInPage на одной странице,
    // начиная с page номера страницы 
    Task<IEnumerable<Message>> GetMessagesByEventId(int eventId, int countInPage, int page);
    // Возвращает все сообщения, написанные с профиля с profileId в рамках ивента с eventId
    Task<IEnumerable<Message>> GetMessagesByProfileId(int profileId, int eventId);
    Task<bool> UpdateMessage(Message message);
    Task<int> AddMessage(Message message);

}
