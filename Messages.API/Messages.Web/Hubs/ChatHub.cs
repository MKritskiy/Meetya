using Messages.Application.Interfaces;
using Messages.Application.Models;
using Messages.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Messages.Web.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IParticipantService _participantService;

    public ChatHub(IMessageService messageService, IParticipantService participantService)
    {
        _messageService = messageService;
        _participantService = participantService;
    }

    public async Task Send(int eventId, string content, int profileId)
    {
        var message = new Message()
        {
            EventId = eventId,
            ProfileId = profileId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };
        await _messageService.AddMessage(message);
        await Clients.Group(eventId.ToString()).SendAsync("Recieve", message);
    }
    public async Task JoinGroup(int eventId, int profileId)
    {
        var added = await _participantService.AddParticipantToEventAsync(new ParticipantDto() { EventId = eventId, ProfileId = profileId });
        if (added.IsSuccessStatusCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, eventId.ToString());
        }
    }
    public async Task LeaveGroup(int eventId, int profileId)
    {
        var removed = await _participantService.RemoveParicipantFromEventAsync(new ParticipantDto() { EventId = eventId, ProfileId = profileId });
        if (removed.IsSuccessStatusCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId.ToString());
        }
    }
}
