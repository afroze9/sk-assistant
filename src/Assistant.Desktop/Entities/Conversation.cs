namespace Assistant.Desktop.Entities;

public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Message> Messages { get; set; } = [];
    public string? Title { get; set; }
}