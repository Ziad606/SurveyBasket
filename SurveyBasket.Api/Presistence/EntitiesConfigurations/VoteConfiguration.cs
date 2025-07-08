
namespace SurveyBasket.Api.Presistenace.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.PollId }).IsUnique();

    }
}
