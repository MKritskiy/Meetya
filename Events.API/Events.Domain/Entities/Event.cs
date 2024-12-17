using Share.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Events.Domain.Entities;

[Table("Event")]
public class Event : BaseEntity
{
    [Required]
    public int CreatorId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Place { get; set; }

    public DateTime? Time { get; set; }

    public decimal? Budget { get; set; }

    public int? PeopleCount { get; set; }

    public string? Description { get; set; }

    public string? PhotoPreview { get; set; }

    public string? Address { get; set; }

    public bool Draft { get; set; } = false;

    public bool PollEnabled { get; set; } = false;

    public bool GeoEnabled { get; set; } = false;

    public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public ICollection<Poll> Polls { get; set; } = new List<Poll>();

}
