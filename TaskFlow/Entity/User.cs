namespace TaskFlow.Entity;

public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public List<Todo>? Todos { get; private set; }

    private User(string Email, string FullName)
    {
        this.Email = Email;
        this.FullName = FullName;
    }

    public static User Create(string Email, string FullName)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(FullName))
            throw new ArgumentException("Invalid Data!\nCheck your Email\nor\nCheck your Name");

        return new User(Email: Email, FullName: FullName);
    }

    public Todo CreateUserTodo(
        string title,
        string? description,
        DateTime endOfDate,
        bool? isComplete
    )
    {
        if (string.IsNullOrWhiteSpace(description))
            description = "Not Assigned Value Yet";

        if (endOfDate <= DateTime.UtcNow)
            throw new ArgumentException("End date must be in the future.");

        var todo = Todo.Create(title, description, endOfDate, isComplete);

        Todos ??= [];
        Todos.Add(todo);

        return todo;
    }

    // public void ChangeFullName(string name)...

    // public void AddTodo(Todo todo)
    // {
    //     Todos?.Add(todo);
    // }
}
