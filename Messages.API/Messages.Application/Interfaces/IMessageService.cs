using Messages.Application.Models;
using Messages.Domain.Entities;

namespace Messages.Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto> GetMessage(int id);
    Task DeleteMessage(int id);
    // Возвращает сообщения для ивента с id равным eventId
    // В количестве countInPage на одной странице,
    // начиная с page номера страницы 
    Task<IEnumerable<MessageDto>> GetMessagesByEventId(int eventId, int countInPage, int page);
    // Возвращает все сообщения, написанные с профиля c profileId в рамках ивента с eventId
    Task<IEnumerable<MessageDto>> GetMessagesByProfileId(int profileId, int eventId);
    Task<bool> UpdateMessage(Message message);
    Task<int> AddMessage(Message message);

}
