
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        //var user = context.User.Identity;
        //if (user is null)
        //    return;

        //if (!user.IsAuthenticated)
        //    return;

        //if (!context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
        //    return;

        if (context.User.Identity is not { IsAuthenticated: true } ||
            !context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
            return;

        context.Succeed(requirement);
        return;

    }
}
