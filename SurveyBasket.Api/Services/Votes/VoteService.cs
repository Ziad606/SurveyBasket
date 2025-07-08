using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services.Votes;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken)
    {
        var hasVote = await _context.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteAlreadyVoted);

        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow) && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var requestQuestionIds = request.Answers
            .Select(a => a.QuestionId);

        var validQuestionIds = await _context.Questions
            .Where(q => q.PollId == pollId && q.IsActive)
            .AsNoTracking()
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        var allQuestionsValid = requestQuestionIds.SequenceEqual(validQuestionIds);
        if (!allQuestionsValid)
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<List<VoteAnswer>>()
        };

        await _context.Votes.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
