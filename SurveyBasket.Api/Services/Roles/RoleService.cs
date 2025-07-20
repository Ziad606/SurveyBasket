using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services.Roles;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled, CancellationToken cancellationToken) =>
        await _roleManager.Roles
        .Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id, CancellationToken cancellationToken)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(p => p.Value));

        return Result.Success(response);

    }

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request, CancellationToken cancellationToken)
    {
        var roleIsExists = await _roleManager.RoleExistsAsync(request.Name);
        if (roleIsExists)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();
        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            var permissions = request.Permissions
                .Select(permission => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = permission
                })
                .ToList();

            await _context.AddRangeAsync(permissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, request.Permissions);
            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> UpdateAsync(string id, RoleRequest request, CancellationToken cancellationToken)
    {
        var roleIsExists = await _context.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id, cancellationToken);

        if (roleIsExists)
            return Result.Failure(RoleErrors.DuplicatedRole);

        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);


        var allowedPermissions = Permissions.GetAllPermissions();
        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var existingPermissions = await _context.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Permissions.Type)
                .Select(p => p.ClaimValue)
                .ToListAsync(cancellationToken);

            var newPermissions = request.Permissions
                .Except(existingPermissions)
                .Select(permission => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = permission
                });

            var removedPermissions = existingPermissions
                .Except(request.Permissions);

            await _context.AddRangeAsync(newPermissions, cancellationToken);

            await _context.RoleClaims
                .Where(p => p.RoleId == id && removedPermissions.Contains(p.ClaimValue))
                .ExecuteDeleteAsync(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }


    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;
        var result = await _roleManager.UpdateAsync(role);
        return Result.Success();
    }
}
