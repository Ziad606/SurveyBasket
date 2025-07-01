using SurveyBasket.Api.Services.Authentication;

namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {

        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);


        return authResult is null ? BadRequest("Invalid Email or Password") : Ok(authResult);
    }

}
