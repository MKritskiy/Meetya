using Share.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Messages.Domain.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = null!;
    [Required]
    public int ProfileId { get; set; }
    public DateTime Timestamp { get; set; }
    [Required]
    public int EventId { get; set; }
}
