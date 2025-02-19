using Messages.Application.Event;
using Messages.Application.Interfaces;
using Messages.Application.Models;
using Messages.Application.Users;
using Messages.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Messages.Web.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IEventsClientApi _eventsClientApi;
    private readonly IUsersApiClient _usersApiClient;

    public ChatHub(IMessageService messageService, IEventsClientApi eventsClientApi, IUsersApiClient usersApiClient)
    {
        _messageService = messageService;
        _eventsClientApi = eventsClientApi;
        _usersApiClient = usersApiClient;
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
        var profile = await _usersApiClient.ApiProfileAsync(profileId);
        await Clients.Group(eventId.ToString()).SendAsync(
            $"Receive_{eventId}", 
            new MessageDto() { 
                Message = message, 
                Profile = profile
            });
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
