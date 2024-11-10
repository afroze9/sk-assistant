namespace Assistant.Desktop.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public required string Content { get; set; }
    public required ChatMessageRole Role { get; set; }
    
    public enum ChatMessageRole
    {
        System = 0,
        User = 1,
        Assistant = 2,
        Tool = 3,
    }
}