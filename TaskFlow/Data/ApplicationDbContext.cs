using Microsoft.EntityFrameworkCore;
using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService
    )
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // old code: t => t.UserId == _currentUserService.UserId
        // The problem: _currentUserService.UserId returns int? (nullable), but UserId is int (non-nullable).
        // When the user is not authenticated, UserId is null → EF translates `t.UserId == null` to `FALSE` in SQL
        // (since an int can never be null), blocking ALL results even for authenticated users.
        // Fix: Only apply the filter when UserId has a value. The `&&` short-circuits:
        //   - If UserId is null → `false && ...` → evaluates to false → no rows returned (secure).
        //   - If UserId has value → evaluates the real condition → filters by user.
        modelBuilder
            .Entity<Todo>()
            .HasQueryFilter(t =>
                _currentUserService.UserId != null && t.UserId == _currentUserService.UserId.Value
            );
    }
}
