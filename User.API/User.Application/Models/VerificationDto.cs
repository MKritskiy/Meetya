namespace Users.Application.Models;

public class VerificationDto
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
