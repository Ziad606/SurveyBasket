namespace SurveyBasket.Api.Contracts.Votes;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(x => x.AnswerId)
            .GreaterThan(0)
            .WithMessage("All answer IDs must be greater than zero.");

        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("All question IDs must be greater than zero.");
    }
}
