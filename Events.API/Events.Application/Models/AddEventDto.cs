using Events.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Events.Application.Models;

public class AddEventDto
{
    [Required]
    public int CreatorId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Place { get; set; }

    public DateTime? Time { get; set; } = DateTime.UtcNow;

    public decimal? Budget { get; set; }

    public int? PeopleCount { get; set; }

    public string? Description { get; set; }

    public string? PhotoPreview { get; set; }

    public string? Address { get; set; }

    public bool Draft { get; set; } = false;

    public bool PollEnabled { get; set; } = false;

    public bool GeoEnabled { get; set; } = false;
    public Event ToEvent() => new Event()
    {
        CreatorId = CreatorId,
        Name = Name,
        Type = Type,
        Place = Place,
        Time = Time,
        Budget = Budget,
        PeopleCount = PeopleCount,
        Description = Description,
        PhotoPreview = PhotoPreview,
        Address = Address,
        Draft = Draft,
        PollEnabled = PollEnabled,
        GeoEnabled = GeoEnabled,

    };
}
