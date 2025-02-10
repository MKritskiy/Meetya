using Friends.Application.Interfaces;
using Friends.Domain.Entities;
using Friends.Infrastructure.Data;
using Infrastructure.Repositories;

namespace Friends.Infrastructure.Repositories;

public class FriendRepository : BaseRepository<Friend>, IFriendRepository
{
    public FriendRepository(FriendDbContext context) : base(context)
    {
    }

    public async Task<Friend?> GetFriendByProfileId(int profileId)
    {
        return await _dbSet.Where(f => f.ProfileId == profileId || f.FriendId==profileId).FirstOrDefaultAsync() ?? null;
    }

    public async Task<List<int>> GetFriendsByProfileIdAsync(int profileId)
    {
        return await _dbSet
            .Where(f => f.ProfileId == profileId || f.FriendId==profileId)
            .Select(f => (f.ProfileId == profileId ? f.FriendId : f.ProfileId) ?? 0)
            .ToListAsync();
    }

    protected override int? GetId(Friend entity)
    {
        return entity.ProfileId;
    }
}
