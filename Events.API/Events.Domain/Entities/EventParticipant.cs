
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Events.Domain.Entities;

public class EventParticipant
{
    [Key, Column(Order = 0)]
    public int EventId { get; set; }

    [Key, Column(Order = 1)]
    public int ProfileId { get; set; }

    [ForeignKey("EventId")]
    public Event Event { get; set; }
}
