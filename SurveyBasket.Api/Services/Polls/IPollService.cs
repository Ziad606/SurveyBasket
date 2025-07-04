namespace SurveyBasket.Api.Services.Polls;

public interface IPollService
{


    Task<Result<IEnumerable<PollResponse>>> GetALlAsync(CancellationToken cancellationToken = default);

    Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<PollResponse> AddAsync(PollRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<Result> TogglePublishStatusASync(int id, CancellationToken cancellationToken = default);
}
