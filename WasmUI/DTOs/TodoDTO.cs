namespace WasmUI.DTOs;

public class TodoDTO
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EndOfDate { get; set; }
    public bool IsComplete { get; set; }
}
