using SurveyBasket.Api.Contracts.Results;

namespace SurveyBasket.Api.Services.Results;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);
        if (!pollIsExists)
            return Result.Failure<PollVotesResponse>(ResultErrors.PollNotFound);

        var pollVotes = await _context.Polls
            .Where(p => p.Id == pollId)
            .Select(p => new PollVotesResponse(
                p.Title,
                p.Votes.Select(v => new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(va => new QuestionAnswerResponse(
                        va.Question.Content,
                        va.Answer.Content
                    ))
                ))
            ))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return pollVotes is not null
            ? Result.Success(pollVotes)
            : Result.Failure<PollVotesResponse>(ResultErrors.PollNotFound);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);
        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(ResultErrors.PollNotFound);

        var votesPerDay = await _context.Votes
            .Where(v => v.PollId == pollId)
            .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SubmittedOn) })
            .Select(g => new VotesPerDayResponse(
                g.Key.Date,
                g.Count()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);
        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(ResultErrors.PollNotFound);

        var votesPerQuestion = await _context.Votes
            .Where(v => v.PollId == pollId)
            .SelectMany(v => v.VoteAnswers)
            .Select(va => new VotesPerQuestionResponse(
                va.Question.Content,
                va.Question.Votes.GroupBy(va => va.Answer.Content)
                 .Select(g => new VotesPerAnswerResponse(
                     g.Key,
                     g.Count()
                 ))
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
    }
}
