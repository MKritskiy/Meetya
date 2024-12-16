
using Friends.Application.Models;

namespace Friends.Application.Interfaces;

public interface IFriendService
{
    Task<int> AddFriend(Friend friend);
    Task DeleteFriend(int profileId, int friendId);
    Task<Friend> GetFriendByProfileId(int friendId);
    Task<UserFriendDto> GetFriendsByProfileId(int userid);
}
