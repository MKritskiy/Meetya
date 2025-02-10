using Friends.Application;
using Friends.Application.Interfaces;
using Friends.Application.Models;
using Friends.Domain.Entities;

namespace Friends.Infrastructure.Services;

class FriendService : IFriendService
{
    IFriendRepository _friendRepository;
    IUsersApiClient _usersApiClient;

    public FriendService(IFriendRepository friendRepository, IUsersApiClient usersApiClient)
    {
        _friendRepository = friendRepository;
        _usersApiClient = usersApiClient;
    }

    public async Task<int> AddFriend(Friend friend)
    {
        return await _friendRepository.AddAsync(friend) ?? 0;
    }

    public async Task DeleteFriend(int profileId, int friendId)
    {
        bool res = await _friendRepository.DeleteByIdAsync(profileId, friendId);
        if (!res) throw new InvalidOperationException();
    }

    public async Task<Friend> GetFriendByProfileId(int profileId)
    {
        Friend res = await _friendRepository.GetFriendByProfileId(profileId) ?? new Friend();
        if (res.ProfileId==null) throw new InvalidOperationException();
        return res;
    }

    public async Task<UserFriendDto> GetFriendsByProfileId(int profileId)
    {
        var friend = await GetFriendByProfileId(profileId);
        if (friend.ProfileId==null) throw new InvalidOperationException();
        var friendsProfilesIds = await _friendRepository.GetFriendsByProfileIdAsync(profileId);
        UserFriendDto userFriendDto = new UserFriendDto()
        {
            Profile = await _usersApiClient.ProfileAsync(profileId),
            FriendsProfiles = await _usersApiClient.ProfilesAsync(friendsProfilesIds)
        };
        return  userFriendDto;
    }
}
