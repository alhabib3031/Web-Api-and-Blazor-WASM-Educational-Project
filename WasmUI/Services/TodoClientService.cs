using System.Net.Http.Json;
using WasmUI.DTOs;
using WasmUI.Services.Interfaces;

namespace WasmUI.Services;

public class TodoClientService : ITodoClientService
{
    private readonly HttpClient _http;
    private const string Endpoint = "task";

    public TodoClientService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TodoItemDTO>?> GetTodosAsync(
        CancellationToken cancellationToken = default
    ) => await _http.GetFromJsonAsync<List<TodoItemDTO>>(Endpoint, cancellationToken) ?? [];

    public async Task<TodoItemDTO?> GetTodosAsync(
        int id,
        CancellationToken cancellationToken = default
    ) => await _http.GetFromJsonAsync<TodoItemDTO>($"{Endpoint}/{id}", cancellationToken);

    public async Task CreateAsync(TodoItemDTO dto, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _http.PostAsJsonAsync(
            Endpoint,
            dto,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(
        int id,
        TodoItemDTO dto,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await _http.PutAsJsonAsync(
            $"{Endpoint}/{id}",
            dto,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _http.DeleteAsync(
            $"{Endpoint}/{id}",
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }
}
