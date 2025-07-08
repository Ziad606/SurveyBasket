namespace SurveyBasket.Api.Errors;

public static class VoteErrors
{
    public static readonly Error VoteNotFound = new("Vote.NotFound", "Vote Not Found", StatusCodes.Status404NotFound);
    public static readonly Error VoteAlreadyVoted = new("Vote.VoteAlreadyVoted", "Vote Already Voted", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidQuestions = new("Vote.InvalidQuestion", "Invalid Questions", StatusCodes.Status400BadRequest);

}

