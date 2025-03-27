using Events.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Events.Application.Models;

public class AddPollDto
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public int ProfileId { get; set; }

    public string PreferredDates { get; set; }

    public Poll ToPoll() => new Poll()
    {
        EventId = EventId,
        ProfileId = ProfileId,
        PreferredDates = PreferredDates,
    };
    public Poll UpdatePollFields(Poll poll)
    {
        poll.EventId = EventId;
        poll.ProfileId = ProfileId;
        poll.PreferredDates = PreferredDates;
        return poll;
    }
}
