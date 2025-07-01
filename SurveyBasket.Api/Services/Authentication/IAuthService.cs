namespace SurveyBasket.Api.Services.Authentication;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken CancellationToken = default);
}
