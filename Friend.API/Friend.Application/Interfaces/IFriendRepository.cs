
using Friends.Application.Models;

namespace Friends.Application.Interfaces;

public interface IFriendRepository : IBaseRepository<Friend>
{
    Task<List<int>> GetFriendsByProfileIdAsync(int profileId);
    Task<Friend?> GetFriendByProfileId(int profileId);
}
