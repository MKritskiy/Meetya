

namespace Users.Domain.Entities
{
    public class User : BaseEntity
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public string Salt { get; set; } = null!;
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;
        public Profile? Profile { get; set; }
        public string Role {  get; set; } = Roles.User;
    }
}
