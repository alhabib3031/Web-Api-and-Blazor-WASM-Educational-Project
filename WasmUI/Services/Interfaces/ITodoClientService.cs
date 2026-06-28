using WasmUI.DTOs;

namespace WasmUI.Services.Interfaces;

public interface ITodoClientService
{
    public Task<List<TodoDTO>?> GetTodosAsync(CancellationToken cancellationToken = default);
    public Task<TodoDTO?> GetTodosAsync(int Id, CancellationToken cancellationToken = default);
    public Task CreateAsync(TodoDTO dto, CancellationToken cancellationToken = default);
    public Task UpdateAsync(int id, TodoDTO dto, CancellationToken cancellationToken = default);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
