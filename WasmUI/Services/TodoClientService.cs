using System.Net.Http.Json;
using WasmUI.DTOs;

namespace WasmUI.Services;

public class TodoClientService
{
    private readonly HttpClient _http;

    public TodoClientService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TodoItemDTO>?> GetTodosAsync()
    {
        return await _http.GetFromJsonAsync<List<TodoItemDTO>>("task");
    }
}
