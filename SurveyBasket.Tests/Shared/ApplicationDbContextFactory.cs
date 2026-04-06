using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SurveyBasket.Api.Presistenace;
using System.Security.Claims;

namespace SurveyBasket.Tests.Shared;

public static class ApplicationDbContextFactory
{
    public static ApplicationDbContext CreateContext(string userId = "test-user-id")
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var httpContextAccessor = SetupHttpContextAccessor(userId);
        return new ApplicationDbContext(options, httpContextAccessor);
    }

    private static IHttpContextAccessor SetupHttpContextAccessor(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };


        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        return httpContextAccessor.Object;
    }
}