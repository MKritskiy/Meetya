namespace Users.Domain.Entities;

public class Profile : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public string About { get; set; } = string.Empty;
    public string CurrentAvatar { get; set; } = string.Empty;
    public DateTimeOffset LastActivity { get; set; }
    public int Status { get; set; }
    public int UserId { get; set; }
}
