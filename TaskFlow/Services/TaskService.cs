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

    public async Task<List<Todo>> GetAll()
    {
        var items = await _context.Todos.ToListAsync();
        return items;
    }

    public async Task<Todo?> GetById(int Id)
    {
        var item = await _context.FindAsync<Todo>(Id);

        if (item is not null)
        {
            return item;
        }

        return null;
    }

    public async Task Create(Todo Item)
    {
        _context.Add(Item);

        await _context.SaveChangesAsync();
    }

    public async Task Update(int Id, Todo Item)
    {
        var item = await _context.FindAsync<Todo>(Id);

        if (item is not null)
        {
            item.Title = Item.Title;
            item.IsComplete = Item.IsComplete;

            await _context.SaveChangesAsync();
        }
        if (item is null)
            Console.WriteLine("this is invalid id");
    }

    public async Task UpdatePatch(int Id, Todo Item)
    {
        var item = await _context.FindAsync<Todo>(Id);

        if (item is not null)
        {
            if (Item.Title is not null)
                item.Title = Item.Title;

            if (Item.IsComplete is not null)
                item.IsComplete = Item.IsComplete;

            await _context.SaveChangesAsync();
        }
        if (item is null)
            Console.WriteLine("this is invalid id");
    }

    public async Task<bool> Delete(int Id)
    {
        var item = await _context.FindAsync<Todo>(Id);

        if (item is not null)
        {
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
