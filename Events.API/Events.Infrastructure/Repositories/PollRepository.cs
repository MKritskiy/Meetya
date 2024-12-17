using Events.Application.Interfaces;
using Events.Domain.Entities;
using Events.Infrastructure.Data;
using Infrastructure.Repositories;

namespace Events.Infrastructure.Repositories;

public class PollRepository : BaseRepository<Poll>, IPollRepository
{
    public PollRepository(EventDbContext context) : base(context)
    {
    }

    public async Task<Poll?> GetPollByProfileId(int profileId)
    {
        return await _dbSet.Where(p=> p.ProfileId == profileId).FirstOrDefaultAsync();
    }

    protected override int? GetId(Poll entity)
    {
        return entity.Id;
    }
}
