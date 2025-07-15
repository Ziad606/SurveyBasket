using SurveyBasket.Api.Contracts.User;
using SurveyBasket.Api.Extensions;
using SurveyBasket.Api.Services.User;

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info(CancellationToken cancellationToken)
    {
        var result = await _userService.GetProfileAsync(User.GetUserId()!, cancellationToken);

        return Ok(result.Value);
    }

    [HttpPut("info")]
    public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateProfileAsync(User.GetUserId()!, request, cancellationToken);

        return NoContent();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> Info([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
