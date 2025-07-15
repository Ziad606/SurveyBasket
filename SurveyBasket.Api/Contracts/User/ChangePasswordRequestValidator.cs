using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.User;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must meet the following requirements:\r\n- At least 8 characters long\r\n- At least one uppercase letter (A-Z)\r\n- At least one lowercase letter (a-z)\r\n- At least one number (0-9)\r\n- At least one special character (!@#$%^&* etc.)")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password cannot equal current password");
    }
}
