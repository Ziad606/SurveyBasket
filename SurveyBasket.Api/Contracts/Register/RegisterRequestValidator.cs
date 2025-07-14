using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must meet the following requirements:\r\n- At least 8 characters long\r\n- At least one uppercase letter (A-Z)\r\n- At least one lowercase letter (a-z)\r\n- At least one number (0-9)\r\n- At least one special character (!@#$%^&* etc.)");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

    }
}
