
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Presistenace.EntitiesConfigurations;

public class UsserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {

        builder.HasData([new IdentityUserRole<string>
            {
                RoleId = DefaultRole.AdminRoleId,
                UserId = DefaultUser.AdminId,

            }
        ]);
    }
}
