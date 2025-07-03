using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Api.Presistenace;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Poll> Polls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
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
