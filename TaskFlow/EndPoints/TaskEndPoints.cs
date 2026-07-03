using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.EndPoints;

public static class TaskEndPoints
{
    public static WebApplication MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/task").RequireAuthorization();

        group.MapGet(
            "",
            async (ITaskService service) =>
            {
                var tasks = await service.GetAll();
                return Results.Ok(tasks);
            }
        );

        group.MapGet(
            "/{id}",
            async (int id, ITaskService service) =>
            {
                var task = await service.GetById(id);
                return Results.Ok(task);
            }
        );

        group.MapPost(
            "",
            async (
                Todo todo,
                ITaskService service,
                IGoogleCalendarService googleCalendarService,
                ICurrentUserService currentUserService,
                HttpContext context
            ) =>
                await CreateTodoAndAddToCalendar(
                    todo,
                    service,
                    googleCalendarService,
                    currentUserService,
                    context
                )
        );

        group.MapPut(
            "/{id}",
            async (int id, Todo todo, ITaskService service) =>
            {
                await service.Update(id, todo);
                return Results.NoContent();
            }
        );

        group.MapPatch(
            "/{id}",
            async (int id, Todo todo, ITaskService service) =>
            {
                await service.UpdatePatch(id, todo);
                return Results.NoContent();
            }
        );

        group.MapDelete(
            "/{id}",
            async (int id, ITaskService service) =>
            {
                var deleted = await service.Delete(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            }
        );

        return app;
    }

    public static async Task<IResult> CreateTodoAndAddToCalendar(
        Todo todo,
        ITaskService service,
        IGoogleCalendarService googleCalendarService,
        ICurrentUserService currentUserService,
        HttpContext context
    )
    {
        try
        {
            todo.StartDateTime = todo.StartDateTime.ToUniversalTime();
            todo.EndDateTime = todo.EndDateTime.ToUniversalTime();

            await service.Create(todo);

            var userId = currentUserService.UserId;
            if (userId.HasValue)
            {
                var (success, message) = await googleCalendarService.AddEventAsync(
                    userId.Value,
                    todo.Title,
                    todo.Description ?? "",
                    todo.StartDateTime,
                    todo.EndDateTime
                );

                context.Response.Headers.Append("X-Calendar-Sync", success ? "synced" : "failed");
                context.Response.Headers.Append("X-Calendar-Message", message);
            }
            else
            {
                context.Response.Headers.Append("X-Calendar-Sync", "failed");
                context.Response.Headers.Append(
                    "X-Calendar-Message",
                    "No connection with Google Calendar"
                );
            }

            return Results.Created(
                $"/task/{todo.Id}",
                new
                {
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.StartDateTime,
                    todo.EndDateTime,
                    todo.IsComplete,
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL POST TASK ERROR]: {ex}");
            context.Response.Headers.Append("X-Calendar-Sync", "failed");
            context.Response.Headers.Append("X-Calendar-Message", ex.Message);
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
    }
}
