namespace SurveyBasket.Api.Services.Polls;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetALlAsync(CancellationToken cancellationToken)
    {
        return await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Polls.FindAsync(id, cancellationToken);
    }

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken)
    {
        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken)
    {
        var existingPoll = await GetAsync(id, cancellationToken);

        if (existingPoll is null)
            return false;

        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.StartsAt = poll.StartsAt;
        existingPoll.EndsAt = poll.EndsAt;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existingPoll = await GetAsync(id, cancellationToken);

        if (existingPoll is null)
            return false;

        _context.Remove(existingPoll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> TogglePublishStatusASync(int id, CancellationToken cancellationToken)
    {
        var existingPoll = await GetAsync(id, cancellationToken);

        if (existingPoll is null)
            return false;

        existingPoll.IsPublished = !existingPoll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
