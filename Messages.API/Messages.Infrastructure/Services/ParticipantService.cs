using Messages.Application.Interfaces;
using Messages.Application.Models;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Messages.Infrastructure.Services;

public class ParticipantService : IParticipantService
{
    private readonly HttpClient _httpClient;

    public ParticipantService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> AddParticipantToEventAsync(ParticipantDto participant)
    {
        return await _httpClient.PostAsJsonAsync("api/participant/add", participant);
    }

    public async Task<HttpResponseMessage> RemoveParicipantFromEventAsync(ParticipantDto participant)
    {
        return await _httpClient.DeleteAsync($"api/participant/remove/{participant.EventId}/{participant.ProfileId}");
    }
}
