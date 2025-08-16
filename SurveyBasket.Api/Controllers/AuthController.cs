using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Contracts.Register;
using SurveyBasket.Api.Contracts.User;
using SurveyBasket.Api.Services.Authentication;

namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
// [EnableRateLimiting(RateLimitPolicies.IpLimiter)]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;


    [HttpPost("")]
    [EnableRateLimiting("userLimiter")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);


        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);


        return isRevoked.IsSuccess ? Ok() : isRevoked.ToProblem();
    }

    [HttpPost("register")]
    [DisableRateLimiting]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);


        return result.IsSuccess ? Ok() : result.ToProblem();
    }


    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request)
    {
        var result = await _authService.ResendConfirmEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.SendResetPasswordCodeAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }


    [HttpGet("test")]
    [EnableRateLimiting(RateLimitPolicies.Concurrency)]
    // [EnableRateLimiting(RateLimitPolicies.TokenBucket)]
    // [EnableRateLimiting(RateLimitPolicies.FixedWindow)]
    // [EnableRateLimiting(RateLimitPolicies.SlidingWindow)]
    public IActionResult Test()
    {
        Thread.Sleep(10000);
        return Ok();
    }
}

