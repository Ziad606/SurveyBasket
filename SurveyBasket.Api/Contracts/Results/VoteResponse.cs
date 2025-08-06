namespace SurveyBasket.Api.Contracts.Results;

public record VoteResponse(
    //string VoterName, // TODO
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
);