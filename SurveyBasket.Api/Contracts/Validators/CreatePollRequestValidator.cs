namespace SurveyBasket.Api.Contracts.Validators;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title)
             .NotEmpty()
             .WithMessage("{PropertyName} is required.")
             .Length(3, 100)
             .WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters long, You Entered [{PropertyValue}]");
    }
}
