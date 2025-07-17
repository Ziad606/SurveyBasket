using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Api.Presistenace;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) :
    IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Answer> Answers { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswer> VoteAnswers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        foreach (var entryEntity in entries)
        {
            if (entryEntity.State == EntityState.Added)
            {
                entryEntity.Property(x => x.CreatedById).CurrentValue = currentUserId;


            }
            if (entryEntity.State == EntityState.Modified)
            {
                entryEntity.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entryEntity.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }


        return base.SaveChangesAsync(cancellationToken);
    }
}
