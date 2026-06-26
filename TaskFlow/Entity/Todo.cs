namespace TaskFlow.Entity;

public class Todo
{
    public int Id { get; private set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime EndOfDate { get; set; }
    public bool? IsComplete { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    // Id is auto Genarated in DB
    private Todo(string Title, string? Description, DateTime EndOfDate, bool? IsComplete)
    {
        this.Title = Title;
        this.Description = Description;
        this.EndOfDate = EndOfDate;
        this.IsComplete = IsComplete;
    }

    internal static Todo Create(
        string title,
        string? description,
        DateTime endOfDate,
        bool? isComplete
    )
    {
        if (string.IsNullOrWhiteSpace(description))
            description = "Not Assigned Value Yet";

        return new Todo(
            Title: title,
            Description: description,
            EndOfDate: endOfDate,
            IsComplete: isComplete
        );
    }
}
