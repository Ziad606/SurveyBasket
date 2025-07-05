namespace SurveyBasket.Api.Contracts.Polls;

public class LoginRequestValidator : AbstractValidator<PollRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Title)
             .NotEmpty()
             .WithMessage("{PropertyName} is required.")
             .Length(3, 100)
             .WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters long, You Entered [{PropertyValue}]");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(x => x)
            .Must(HasValidDate)
            .WithName(nameof(PollRequest.EndsAt))
            .WithMessage("Start Date should be before {PropertyName}");
    }


    private bool HasValidDate(PollRequest pollRequest)
    {
        return pollRequest.EndsAt >= pollRequest.StartsAt;
    }
}
