namespace SurveyBasket.Api.Contracts.User;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);


        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required and must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must meet the following requirements:\r\n- At least 8 characters long\r\n- At least one uppercase letter (A-Z)\r\n- At least one lowercase letter (a-z)\r\n- At least one number (0-9)\r\n- At least one special character (!@#$%^&* etc.)");


        RuleFor(x => x.Roles)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Roles)
            .Must(r => r.Distinct().Count() == r.Count)
            .WithMessage("You cannot add duplicated roles")
            .When(r => r.Roles != null);


    }
}
