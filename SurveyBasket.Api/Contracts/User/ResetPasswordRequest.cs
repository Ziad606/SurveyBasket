namespace SurveyBasket.Api.Contracts.User;

public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword
);
