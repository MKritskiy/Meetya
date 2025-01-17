using Users.Domain.Entities;

namespace Users.Application.Models;

public class AddProfileDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public string About { get; set; } = string.Empty;
    public string CurrentAvatar { get; set; } = string.Empty;
    public int Status { get; set; }
    public int UserId { get; set; } 
    public Profile ToProfileModel() 
        => new Profile
        {
            About = About,
            Age = Age,
            CurrentAvatar = CurrentAvatar,
            Status = Status,
            FirstName = FirstName,
            LastName = LastName,
            Gender = Gender,
            UserId = UserId,
        };
}
