namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email/password");
    public static readonly Error InvalidToken = new("User.InvalidToken", "Invalid token");
    public static readonly Error InvalidUserId = new("User.Unknown", "Invalid User Id");
}
