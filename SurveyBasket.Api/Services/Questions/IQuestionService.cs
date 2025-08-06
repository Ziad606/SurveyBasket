using SurveyBasket.Api.Contracts;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services.Questions;

public interface IQuestionService
{
    Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilter requestFilter, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, CancellationToken cancellationToken = default); // string userId,
    Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);

}
