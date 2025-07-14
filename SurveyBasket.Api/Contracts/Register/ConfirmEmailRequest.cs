namespace SurveyBasket.Api.Contracts.Register;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);