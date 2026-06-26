using TaskFlow.Entity;
using TaskFlow.Services.Interfaces;

namespace TaskFlow.EndPoints;

public static class TaskEndPoints
{
    public static WebApplication MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/task");

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
