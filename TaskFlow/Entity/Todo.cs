namespace TaskFlow.Entity;

public class Todo
{
    public int Id { get; private set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }

    public bool IsComplete { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public Todo() { }

    // Id is auto Genarated in DB
    private Todo(
        string Title,
        string? Description,
        DateTimeOffset StartDateTime,
        DateTimeOffset EndDateTime,
        bool IsComplete
    )
    {
        this.Title = Title;
        this.Description = Description;
        this.StartDateTime = StartDateTime;
        this.EndDateTime = EndDateTime;
        this.IsComplete = IsComplete;
    }

    internal static Todo Create(
        string title,
        string? description,
        DateTimeOffset StartDateTime,
        DateTimeOffset EndDateTime,
        bool isComplete
    )
    {
        if (string.IsNullOrWhiteSpace(description))
            description = "Not Assigned Value Yet";

        return new Todo(
            Title: title,
            Description: description,
            StartDateTime: StartDateTime,
            EndDateTime: EndDateTime,
            IsComplete: isComplete
        );
    }
}
