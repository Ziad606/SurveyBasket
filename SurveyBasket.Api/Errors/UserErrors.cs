namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status404NotFound);
    public static readonly Error DisabledUser = new("User.DisabledUser", "Disabled user, please contact your adminstrator", StatusCodes.Status404NotFound);
    public static readonly Error EmailNotConfirmed = new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidToken = new("User.InvalidToken", "Invalid token", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidUserId = new("User.Unknown", "Invalid User Id", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicatedEmail = new("User.DuplicatedEmail", "User with the same email already registered", StatusCodes.Status409Conflict);
    public static readonly Error DuplicatedConfirm = new("User.DuplicatedConfirm", "Email already confirmed", StatusCodes.Status409Conflict);
    public static readonly Error InvalidCode = new("User.InvalidCode", "Invalid code", StatusCodes.Status400BadRequest);
    public static readonly Error UserLockedOut = new("User.UserLockedOut", "User lockedout due to many invalid sign-ins ", StatusCodes.Status400BadRequest);
    public static readonly Error UserNotFound = new("User.UserNotFound", "User not found", StatusCodes.Status404NotFound);
    public static readonly Error UserAlreadyExists = new("User.UserAlreadyExists", "User with the same email already exists", StatusCodes.Status409Conflict);
    public static readonly Error InvalidRoles = new("User.InvalidRoles", "Invalid roles", StatusCodes.Status400BadRequest);
}
