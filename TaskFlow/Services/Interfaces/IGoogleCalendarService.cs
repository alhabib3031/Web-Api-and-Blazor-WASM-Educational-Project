using TaskFlow.DTOs;

namespace TaskFlow.Services.Interfaces;

public interface IGoogleCalendarService
{
    Task<List<CalendarEventDto>> GetUpcomingEventsAsync(int userId);
    Task<(bool Success, string Message)> AddEventAsync(
        int userId,
        string title,
        string description,
        DateTimeOffset start,
        DateTimeOffset end
    );
}
