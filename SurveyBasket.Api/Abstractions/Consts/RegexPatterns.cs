namespace SurveyBasket.Api.Abstractions.Consts;

public static class RegexPatterns
{
    public const string Password = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,}$";
}
