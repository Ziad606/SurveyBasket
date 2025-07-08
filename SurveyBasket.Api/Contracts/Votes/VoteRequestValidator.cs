namespace SurveyBasket.Api.Contracts.Votes;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.Answers)
            .NotEmpty()
            .WithMessage("At least one answer is required.");

        //RuleForEach(x => x.Answers)
        //    .Must(answers => answers.AnswerId > 0)
        //    .WithMessage("All answer IDs must be greater than zero.")
        //    .Must(answer => answer.QuestionId > 0)
        //    .WithMessage("All question IDs must be greater than zero.");

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v => v.Add(new VoteAnswerRequestValidator()));
    }
}
