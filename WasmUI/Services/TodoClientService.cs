using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using WasmUI.DTOs;
using WasmUI.Services.Interfaces;

namespace WasmUI.Services;

public class TodoClientService : ITodoClientService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorageService;
    private const string Endpoint = "task";

    public TodoClientService(HttpClient http, ILocalStorageService localStorageService)
    {
        _http = http;
        _localStorageService = localStorageService;
    }

    private async Task AttachTokenAsync()
    {
        var token = await _localStorageService.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
        }
    }

    public async Task<List<TodoDTO>?> GetTodosAsync(CancellationToken cancellationToken = default)
    {
        await AttachTokenAsync();
        var todos = await _http.GetFromJsonAsync<List<TodoDTO>>(Endpoint, cancellationToken) ?? [];
        return todos;
    }

    public async Task<TodoDTO?> GetTodosAsync(int id, CancellationToken cancellationToken = default)
    {
        await AttachTokenAsync();
        var todo = await _http.GetFromJsonAsync<TodoDTO>($"{Endpoint}/{id}", cancellationToken);
        return todo;
    }

    public async Task CreateAsync(TodoDTO dto, CancellationToken cancellationToken = default)
    {
        await AttachTokenAsync();
        using HttpResponseMessage response = await _http.PostAsJsonAsync(
            Endpoint,
            dto,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(
        int id,
        TodoDTO dto,
        CancellationToken cancellationToken = default
    )
    {
        await AttachTokenAsync();
        using HttpResponseMessage response = await _http.PutAsJsonAsync(
            $"{Endpoint}/{id}",
            dto,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await AttachTokenAsync();
        using HttpResponseMessage response = await _http.DeleteAsync(
            $"{Endpoint}/{id}",
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }
}
