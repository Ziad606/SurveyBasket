namespace SurveyBasket.Api.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound = new("Poll.NotFound", "Poll Not Found", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatePollTitle = new("Poll.DuplicatePollTitle", "Poll with the same title already exists", StatusCodes.Status409Conflict);
    public static readonly Error NoAvailablePolls = new("Poll.NoAvailablePolls", "No Available Polls", StatusCodes.Status404NotFound);
    public static readonly Error FailedOperation = new("Poll.FailedOperation", "Failed Operation", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidPollRequest = new("Poll.InvalidPollRequest", "Invalid Poll Request", StatusCodes.Status400BadRequest);
}
