using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services.Roles;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled, CancellationToken cancellationToken);
    Task<Result<RoleDetailResponse>> GetAsync(string id, CancellationToken cancellationToken);
    Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(string id, RoleRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(string id);
}
