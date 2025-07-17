
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Presistenace.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);
        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUser.AdminId,
            FirstName = "Ziad",
            LastName = "Mohammed",
            UserName = DefaultUser.AdminEmail,
            NormalizedUserName = DefaultUser.AdminEmail.ToUpper(),
            Email = DefaultUser.AdminEmail,
            NormalizedEmail = DefaultUser.AdminEmail.ToUpper(),
            EmailConfirmed = true,
            PasswordHash = passwordHasher.HashPassword(null!, DefaultUser.AdminPassword),
            SecurityStamp = DefaultUser.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUser.AdminConcurrencyStamp,
        });
    }
}
