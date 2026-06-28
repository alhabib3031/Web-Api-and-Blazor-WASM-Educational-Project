using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TaskService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        // old code: also injected IConfiguration + used Dapper for GetAll and Create
        // The problem: Dapper bypasses HasQueryFilter (which filters by current user),
        // so GetAll returned ALL todos for ALL users — a major security/data leak.
        // Create() used Dapper but never set UserId, so new todos belonged to UserId = 0.
        // Some methods used EF Core (GetById, Update, Delete), others used Dapper — inconsistent.
        // Fix: Use EF Core everywhere so HasQueryFilter applies consistently to all queries.
        _currentUserService = currentUserService;
    }

    public async Task<List<Todo>> GetAll()
    {
        // old code: Used Dapper raw SQL — bypassed HasQueryFilter, returned all users' todos.
        // Fix: EF Core applies HasQueryFilter automatically, so only the current user's todos are returned.
        return await _context.Todos.OrderBy(t => t.Id).ToListAsync();
    }

    public async Task<Todo?> GetById(int Id)
    {
        // old code: Used _context.FindAsync<Todo>(Id)
        // The problem: FindAsync looks up by primary key directly and DOES NOT apply global query filters,
        // so any authenticated user could access any todo by ID, even if it belonged to another user.
        // Fix: Use FirstOrDefaultAsync with a LINQ expression — this applies HasQueryFilter,
        // so only the current user's todo is returned (or null if it belongs to someone else).
        return await _context.Todos.FirstOrDefaultAsync(t => t.Id == Id);
    }

    public async Task Create(Todo todo)
    {
        // old code: Used Dapper raw SQL — bypassed HasQueryFilter and didn't set UserId.
        // todo.UserId was always 0 (default int) because the endpoint received Todo from the request body
        // without injecting the current user context.
        // Fix: Set UserId from the authenticated user. Throw if not authenticated.
        todo.UserId =
            _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
    }

    public async Task Update(int Id, Todo Item)
    {
        // old code: Used FindAsync — bypassed query filters, allowing cross-user updates.
        // Fix: Use FirstOrDefaultAsync so HasQueryFilter applies.
        var existing = await _context.Todos.FirstOrDefaultAsync(t => t.Id == Id);

        if (existing is not null)
        {
            existing.Title = Item.Title;
            existing.IsComplete = Item.IsComplete;

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePatch(int Id, Todo Item)
    {
        // old code: Used FindAsync — same security issue as Update.
        var existing = await _context.Todos.FirstOrDefaultAsync(t => t.Id == Id);

        if (Item is null)
        {
            return;
        }
        if (existing is not null)
        {
            if (Item.Title is not null)
                existing.Title = Item.Title;

            // if (Item.IsComplete is not null)
            //     existing.IsComplete = Item.IsComplete;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> Delete(int Id)
    {
        // old code: Used FindAsync — same security issue.
        var existing = await _context.Todos.FirstOrDefaultAsync(t => t.Id == Id);

        if (existing is not null)
        {
            _context.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
