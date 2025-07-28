using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasket.Api.Contracts;
using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Questions;
using System.Linq.Dynamic.Core;
namespace SurveyBasket.Api.Services.Questions;

public class QuestionService(ApplicationDbContext context, HybridCache hybridCache, ILogger<QuestionService> logger) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;
    private readonly ILogger<QuestionService> _logger = logger;
    private readonly string _cachePrefix = "availableQuestion";

    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilter filters, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

        var query = _context.Questions
            .Where(q => q.PollId == pollId);

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            query.Where(q => q.Content.Contains(filters.SearchValue));
        }

        if (!string.IsNullOrEmpty(filters.SortCoulms))
        {
            query.OrderBy($"{filters.SortCoulms} {filters.SortDirection}");
        }

        var source = query.Include(q => q.Answers)
        .ProjectToType<QuestionResponse>()
        .AsNoTracking();

        var questions = await PaginatedList<QuestionResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(questions);

    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteAlreadyVoted);

        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow) && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{_cachePrefix}-{pollId}";


        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(cacheKey, async cachEntry =>
                    await _context.Questions
                    .Where(q => q.PollId == pollId && q.IsActive)
                    .Include(q => q.Answers)
                    .Select(q => new QuestionResponse(
                            q.Id,
                            q.Content,
                            q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse
                            (
                                a.Id,
                                a.Content
                            ))
                        ))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken),
                    cancellationToken: cancellationToken);

        return Result.Success(questions);
    }


    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == id)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (question == null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }



    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);

        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(q => q.PollId == pollId && q.Content == request.Content, cancellationToken);
        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicateQuestionContent);

        var question = request.Adapt<Question>();
        question.PollId = pollId;


        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var cacheKey = $"{_cachePrefix}-{pollId}";
        await _hybridCache.RemoveAsync(cacheKey, cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }


    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var quesionIsExists = await _context.Questions
            .AnyAsync(q => q.PollId == pollId
                && q.Id != id
                && q.Content == request.Content,
                cancellationToken);

        if (quesionIsExists)
            return Result.Failure(QuestionErrors.DuplicateQuestionContent);


        var question = await _context.Questions
            .Include(q => q.Answers)
            .SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        var currentAnswers = question.Answers.Select(q => q.Content).ToList();

        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        var cacheKey = $"{_cachePrefix}-{pollId}";
        await _hybridCache.RemoveAsync(cacheKey, cancellationToken);


        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == id, cancellationToken);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        var cacheKey = $"{_cachePrefix}-{pollId}";
        await _hybridCache.RemoveAsync(cacheKey, cancellationToken);


        return Result.Success();
    }

}
