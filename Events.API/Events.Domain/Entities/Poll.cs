using Share.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Events.Domain.Entities;

public class Poll : BaseEntity
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public int ProfileId { get; set; }

    public string PreferredDates { get; set; }

}
