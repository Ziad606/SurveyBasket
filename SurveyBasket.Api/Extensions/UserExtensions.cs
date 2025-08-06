using System.Security.Claims;

namespace SurveyBasket.Api.Extensions;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) => "0198130d-29af-7543-b04f-01652b09f507";
    //user.FindFirstValue(ClaimTypes.NameIdentifier); // TODO

}
