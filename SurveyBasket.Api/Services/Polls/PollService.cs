namespace SurveyBasket.Api.Services.Polls;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<PollResponse>>> GetALlAsync(CancellationToken cancellationToken)
    {
        var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return polls is not null ?
            Result.Success(polls.Adapt<IEnumerable<PollResponse>>()) : Result.Failure<IEnumerable<PollResponse>>(PollErrors.NoAvailablePolls);
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        return poll is not null ?
            Result.Success(poll.Adapt<PollResponse>()) : Result.Failure<PollResponse>(PollErrors.PollNotFound);
    }

    public async Task<PollResponse> AddAsync(PollRequest request, CancellationToken cancellationToken)
    {
        await _context.AddAsync(request.Adapt<Poll>(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return request.Adapt<PollResponse>();
    }

    public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken)
    {
        var existingPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (existingPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.StartsAt = poll.StartsAt;
        existingPoll.EndsAt = poll.EndsAt;
        await _context.SaveChangesAsync(cancellationToken);


        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        _context.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusASync(int id, CancellationToken cancellationToken)
    {
        var existingPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (existingPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        existingPoll.IsPublished = !existingPoll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
