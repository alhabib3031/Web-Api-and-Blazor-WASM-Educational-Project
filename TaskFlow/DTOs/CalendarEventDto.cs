namespace TaskFlow.DTOs;

public class CalendarEventDto
{
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset StartTime { get; set; }
}
