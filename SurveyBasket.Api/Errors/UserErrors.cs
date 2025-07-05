namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status404NotFound);
    public static readonly Error InvalidToken = new("User.InvalidToken", "Invalid token", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidUserId = new("User.Unknown", "Invalid User Id", StatusCodes.Status400BadRequest);
}
