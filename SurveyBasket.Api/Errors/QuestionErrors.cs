namespace SurveyBasket.Api.Errors;

public static class QuestionErrors
{
    public static readonly Error QuestionNotFound = new("Question.NotFound", "Question Not Found", StatusCodes.Status404NotFound);
    public static readonly Error DuplicateQuestionContent = new("Question.DuplicateQuestionContent", "Question with the same content already exists", StatusCodes.Status409Conflict);
    public static readonly Error QuestionAlreadyVoted = new("Question.QuestionAlreadyVoted", "Question Already Voted", StatusCodes.Status400BadRequest);
}
