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
                return Results.Ok();
            }
        );

        return app;
    }
}
