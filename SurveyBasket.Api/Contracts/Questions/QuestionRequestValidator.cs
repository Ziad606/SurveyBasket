namespace SurveyBasket.Api.Contracts.Questions;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull()
            .Must(x => x.Count > 1)
            .WithMessage("Question should has at least 2 answers")
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("you cannot add duplicated answers for the same question");
    }
}
