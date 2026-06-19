using Microsoft.EntityFrameworkCore;
using TaskFlow.Entity;

namespace TaskFlow.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
