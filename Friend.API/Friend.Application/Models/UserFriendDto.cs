namespace Friends.Application.Models
{
    public class UserFriendDto
    {
        public int ProfileId { get; set; }
        public IEnumerable<int> FriendsIds { get; set; } = new List<int>();
    }
}
