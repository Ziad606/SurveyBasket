namespace SurveyBasket.Api.Contracts;

public record RequestFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchValue { get; init; }
    public string? SortCoulms { get; set; }
    public string? SortDirection { get; init; } = "ASC";
}
