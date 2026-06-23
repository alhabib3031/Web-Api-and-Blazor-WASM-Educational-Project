using WasmUI.DTOs;

namespace WasmUI.Services.Interfaces;

public interface ITodoClientService
{
    public Task<List<TodoItemDTO>?> GetTodosAsync(CancellationToken cancellationToken = default);
    public Task<TodoItemDTO?> GetTodosAsync(int Id, CancellationToken cancellationToken = default);
    public Task CreateAsync(TodoItemDTO dto, CancellationToken cancellationToken = default);
    public Task UpdateAsync(int id, TodoItemDTO dto, CancellationToken cancellationToken = default);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
