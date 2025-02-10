using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Friends.Domain.Entities;

public class Friend
{
    [Key, Column(Order = 0)]
    public int? ProfileId { get; set; }

    [Key, Column(Order = 1)]
    public int? FriendId { get; set; }

    public DateTimeOffset Created { get; set; }
}