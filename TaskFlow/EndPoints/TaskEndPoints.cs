using Microsoft.AspNetCore.Authorization;
using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.EndPoints;

public static class TaskEndPoints
{
    public static WebApplication MapTaskEndpoints(this WebApplication app)
    {
        // old code: RequireAuthorization was only on GetAll and Create endpoints.
        // GetById, Update, UpdatePatch, Delete had NO authorization check at all.
        // The problem: Any unauthenticated request could read, modify, or delete any todo.
        // Fix: Apply RequireAuthorization to the entire group so ALL task endpoints require auth.
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
            async (Todo item, ITaskService service) =>
            {
                await service.Create(item);

                return Results.Created($"/task/{item.Id}", item);
            }
        );

        group.MapPut(
            "/{id}",
            async (int id, Todo item, ITaskService service) =>
            {
                await service.Update(id, item);

                return Results.NoContent();
            }
        );

        group.MapPatch(
            "/{id}",
            async (int id, Todo item, ITaskService service) =>
            {
                await service.UpdatePatch(id, item);

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
}
