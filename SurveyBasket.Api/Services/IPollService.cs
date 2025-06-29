namespace SurveyBasket.Api.Services;

public interface IPollService
{


    Task<IEnumerable<Poll>> GetALlAsync(CancellationToken cancellationToken = default);

    Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> TogglePublishStatusASync(int id, CancellationToken cancellationToken = default);
}
