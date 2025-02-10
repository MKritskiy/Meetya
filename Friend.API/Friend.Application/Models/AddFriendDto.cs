namespace Friends.Application.Models;

public class AddFriendDto
{
    public int ProfileId { get; set; }

    public int FriendId { get; set; }

    public Friend ToFriend() =>
        new Friend()
        {
            ProfileId = ProfileId,
            FriendId = FriendId
        };
}
