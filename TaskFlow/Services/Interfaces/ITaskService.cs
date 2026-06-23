using TaskFlow.Entity;

namespace TaskFlow.Services.Interfaces;

public interface ITaskService
{
    public Task<List<TaskItem>> GetAll();
    public Task<TaskItem?> GetById(int Id);
    public Task Create(TaskItem Item);
    public Task Update(int Id, TaskItem Item);
    public Task UpdatePatch(int Id, TaskItem Item);
    public Task<bool> Delete(int Id);
}
