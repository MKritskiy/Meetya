using Messages.Application;
using Messages.Application.Interfaces;
using Messages.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Messages.Web.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IEventsClientApi _eventsClientApi;

    public ChatHub(IMessageService messageService, IEventsClientApi eventsClientApi)
    {
        _messageService = messageService;
        _eventsClientApi = eventsClientApi;
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
        await Clients.Group(eventId.ToString()).SendAsync($"Receive_{eventId}", message);
    }

    public async Task JoinGroup(int eventId, int profileId)
    {
        try
        {
            await _eventsClientApi.ApiParticipantAddAsync(new EventParticipant() { EventId = eventId, ProfileId = profileId });
        }
        catch(Exception ex) 
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine(eventId);
        await Groups.AddToGroupAsync(Context.ConnectionId, eventId.ToString());

    }
    public async Task LeaveGroup(int eventId, int profileId)
    {

         await _eventsClientApi.ApiParticipantRemoveAsync(eventId, profileId);
         await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId.ToString());


    }
}
