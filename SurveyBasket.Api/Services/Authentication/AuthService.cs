
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Register;
using SurveyBasket.Api.Contracts.User;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Api.Services.Authentication;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext context
    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly int _refreshTokenExpiryDays = 14;


    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken CancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (result.Succeeded)
        {
            var (roles, permissions) = await GetUserRolesAndPermissions(user, CancellationToken);
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);
            var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, token, expiresIn, refreshToken, refreshTokenExpiration);
            return Result.Success(response);
        }


        return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);
    }


    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken CancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidUserId);


        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (roles, permissions) = await GetUserRolesAndPermissions(user, CancellationToken);
        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);
        return Result.Success(response);

    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken CancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidUserId);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }


    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation Code : {code}", code);

            await SendConfirmationEmailAsync(user, code);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);


        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirm);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRole.Member);
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation Code : {code}", code);

        await SendConfirmationEmailAsync(user, code);


        return Result.Success();
    }


    public async Task<Result> SendResetPasswordCodeAsync(ForgotPasswordRequest request)
    {

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset Code : {code}", code);

        await SendResetPasswordEmailAsync(user, code);


        return Result.Success();
    }


    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);


        var code = request.Code;

        IdentityResult result;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }


        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));

    }


    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        //var userPermissions = await _context.Roles
        //    .Join(_context.RoleClaims,
        //        role => role.Id,
        //        claim => claim.RoleId,
        //        (role, claim) => new { role, claim })
        //    .Where(x => userRoles.Contains(x.role.Name!))
        //    .Select(x => x.claim.ClaimValue)
        //    .Distinct()
        //    .ToListAsync(cancellationToken);

        var userPermissions = await (from r in _context.Roles
                                     join rc in _context.RoleClaims
                                     on r.Id equals rc.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select rc.ClaimValue)
                                    .Distinct()
                                    .ToListAsync(cancellationToken);

        return (userRoles, userPermissions!);
    }


    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task SendConfirmationEmailAsync(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                    { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}"}
            }
        );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket : Email Confirmation", emailBody));
        await Task.CompletedTask;
    }


    private async Task SendResetPasswordEmailAsync(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                    {"{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/forgetPassword?userId={user.Email}&code={code}"}
            }
        );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket : Change Password", emailBody));
        await Task.CompletedTask;
    }
}
