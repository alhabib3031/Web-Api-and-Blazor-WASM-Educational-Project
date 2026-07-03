using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using WasmUI.DTOs;
using WasmUI.Services.Interfaces;
using static System.Net.WebRequestMethods;

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

    public async Task<TodoDTO?> GetTodosAsync(int id, CancellationToken cancellationToken = default)
    {
        await AttachTokenAsync();
        var todo = await _http.GetFromJsonAsync<TodoDTO>($"{Endpoint}/{id}", cancellationToken);
        return todo;
    }

    public async Task<List<TodoDTO>?> GetTodosAsync(CancellationToken cancellationToken = default)
    {
        await AttachTokenAsync();
        var todos = await _http.GetFromJsonAsync<List<TodoDTO>>(Endpoint, cancellationToken) ?? [];
        return todos;
    }

    public async Task<(bool Success, bool CalendarSynced, string CalendarMessage)> CreateAsync(
        TodoDTO dto,
        CancellationToken cancellationToken = default
    )
    {
        await AttachTokenAsync();
        using HttpResponseMessage response = await _http.PostAsJsonAsync(
            Endpoint,
            dto,
            cancellationToken
        );

        bool success = response.IsSuccessStatusCode;
        bool calendarSynced = false;
        string calendarMessage = "No connection with Google Calendar";

        if (success)
        {
            calendarSynced =
                response.Headers.TryGetValues("X-Calendar-Sync", out var syncValues)
                && syncValues.FirstOrDefault() == "synced";

            if (response.Headers.TryGetValues("X-Calendar-Message", out var msgValues))
            {
                calendarMessage = msgValues.FirstOrDefault() ?? calendarMessage;
            }
        }
        else
        {
            var errorBody = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken: cancellationToken);
            if (errorBody?.TryGetValue("error", out var serverError) == true)
            {
                calendarMessage = serverError;
            }
        }

        return (success, calendarSynced, calendarMessage);
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

    public async Task<List<CalendarEventDto>?> GetUserCalendar(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await AttachTokenAsync();

            var events = await _http.GetFromJsonAsync<List<CalendarEventDto>>(
                "api/calendar",
                cancellationToken
            );
            return events;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error While Gathering the Calendar: {ex.Message}");
            return null;
        }
    }
}
