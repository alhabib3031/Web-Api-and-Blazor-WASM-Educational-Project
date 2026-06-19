using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAll()
    {
        var items = await _context.Tasks.ToListAsync();
        return items;
    }
}
