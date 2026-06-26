using TaskFlow.Entity;

namespace TaskFlow.Services.Interfaces;

public interface ITaskService
{
    public Task<List<Todo>> GetAll();
    public Task<Todo?> GetById(int Id);
    public Task Create(Todo Item);
    public Task Update(int Id, Todo Item);
    public Task UpdatePatch(int Id, Todo Item);
    public Task<bool> Delete(int Id);
}
