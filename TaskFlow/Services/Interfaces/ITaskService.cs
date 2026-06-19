using TaskFlow.Entity;

namespace TaskFlow.Services.Interfaces;

public interface ITaskService
{
    public Task<List<TaskItem>> GetAll();
}
