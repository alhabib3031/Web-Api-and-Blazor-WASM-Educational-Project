using TaskFlow.DTOs;
using TaskFlow.Services;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.EndPoints;

public static class CalendarEndPoints
{
    public static void MapCalendarEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/calendar",
                static async (
                    IGoogleCalendarService calendarService,
                    ICurrentUserService currentUser
                ) =>
                {
                    if (currentUser.UserId is null)
                        return TypedResults.Ok(new List<CalendarEventDto>());

                    var events = await calendarService.GetUpcomingEventsAsync(
                        currentUser.UserId.Value
                    );
                    return TypedResults.Ok(events);
                }
            )
            .RequireAuthorization();
    }
}
