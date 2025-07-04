namespace SurveyBasket.Api.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound = new("Poll.NotFound", "Poll Not Found");
    public static readonly Error NoAvailablePolls = new("Poll.NoAvailablePolls", "No Available Polls");
    public static readonly Error FailedOperation = new("Poll.FailedOperation", "Failed Operation");
    public static readonly Error InvalidPollRequest = new("Poll.InvalidPollRequest", "Invalid Poll Request");
}
