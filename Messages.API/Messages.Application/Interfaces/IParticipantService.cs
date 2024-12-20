
using Messages.Application.Models;

namespace Messages.Application.Interfaces;

public interface IParticipantService
{
    Task<HttpResponseMessage> AddParticipantToEventAsync(ParticipantDto participant);
    Task<HttpResponseMessage> RemoveParicipantFromEventAsync(ParticipantDto participant);
}
