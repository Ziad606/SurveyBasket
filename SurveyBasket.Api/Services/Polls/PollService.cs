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

    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken)
    {
        var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);

        if (isExisting)
            return Result.Failure<PollResponse>(PollErrors.DuplicatePollTitle);
        var poll = request.Adapt<Poll>();
        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken)
    {
        var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);

        if (isExisting)
            return Result.Failure<PollResponse>(PollErrors.DuplicatePollTitle);

        var existingPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (existingPoll is null)
            return Result.Failure(PollErrors.PollNotFound);



        existingPoll.Title = request.Title;
        existingPoll.Summary = request.Summary;
        existingPoll.StartsAt = request.StartsAt;
        existingPoll.EndsAt = request.EndsAt;
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
