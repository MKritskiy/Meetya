namespace Friends.Application.Models
{
    public class UserFriendDto
    {
        public Profile? Profile { get; set; }
        public IEnumerable<Profile> FriendsProfiles { get; set; } = new List<Profile>();
    }
}
